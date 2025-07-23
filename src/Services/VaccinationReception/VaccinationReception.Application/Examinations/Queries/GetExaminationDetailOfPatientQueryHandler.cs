using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetExaminationDetailOfPatientQueryHandler : IQueryHandler<GetExaminationDetailOfPatientQuery, GetExaminationDetailOfPatientQueryResponse>  
    {
        private readonly IApplicationDbContext _context;
        private readonly IHospitalService _hospitalService;
        private readonly IPatientGrpcClient _patientGrpcClient;

        public GetExaminationDetailOfPatientQueryHandler(IApplicationDbContext context, IHospitalService hospitalService, IPatientGrpcClient patientGrpcClient)
        {
            _context = context;
            _hospitalService = hospitalService;
            _patientGrpcClient = patientGrpcClient;
        }

        public async Task<GetExaminationDetailOfPatientQueryResponse> Handle(GetExaminationDetailOfPatientQuery request, CancellationToken cancellationToken)
        {
            var examination = await _context.Examinations
                .Where(e => e.Id == request.ExaminationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (examination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_EXAMINATION_WITH_ID);
            }

            var examinaitonDetail = await _context.ExaminationTestResults
                .Where(e => e.ExaminationId == request.ExaminationId)
                .Select(e => new ExaminationTestParameterOfPatient
                {
                    ParameterName = e.ParameterName,
                    Result = e.ResultValue,
                    StandardValue = e.StandardValue
                })
                .ToListAsync(cancellationToken);

            var services = await _hospitalService.GetServicesByIdsAsync([examination.ServiceId!.Value], cancellationToken);
            var serviceName = services.FirstOrDefault()?.ServiceName ?? string.Empty;

            var patient = await _patientGrpcClient.GetPatientAsync(examination.PatientId!.Value, cancellationToken);

            var response = new GetExaminationDetailOfPatientQueryResponse
            (
                PatientId: examination.PatientId.Value,
                PatientCode: patient.Code,
                PatientName: patient.Name,
                PatientPhoneNumber: patient.PhoneNumber ?? string.Empty,
                ReturnDate: examination.ReturnTime!.Value,
                ServiceName: serviceName,
                Status: examination.ReturnTime < DateTime.UtcNow ? "COMPLETED" : "PENDING",
                ExaminationTestParameters: examinaitonDetail,
                Diagnosis: examination.Diagnose ?? string.Empty,
                Conclusion: examination.Conclusion ?? string.Empty,
                Note: examination.Note ?? string.Empty
            );

            return response;
        }
    }
}

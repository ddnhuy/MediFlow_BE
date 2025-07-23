// ... existing usings ...
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetAllExaminationHistoryOfPatientQueryHandler : IQueryHandler<GetAllExaminationHistoryOfPatientQuery, GetAllExaminationHistoryOfPatientResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;
        private readonly IHospitalService _hospitalService;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllExaminationHistoryOfPatientQueryHandler(IApplicationDbContext context, IHospitalService hospitalService, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto, IPatientGrpcClient patientGrpcClient, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hospitalService = hospitalService;
            _applicationUserProto = applicationUserProto;
            _patientGrpcClient = patientGrpcClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetAllExaminationHistoryOfPatientResponse> Handle(GetAllExaminationHistoryOfPatientQuery request, CancellationToken cancellationToken)
        {
            var examinations = await _context.Examinations
                .Where(e => e.PatientId == request.PatientID)
                .OrderByDescending(e => e.ReturnTime)
                .Select(e => new ExaminationHistoryOfPatientItem
                {
                    ExaminationId = e.Id,
                    ReturnTime = e.ReturnTime ?? DateTime.MinValue,
                    ServiceId = e.ServiceId!.Value,
                    ServiceName = "",
                    DoctorName = e.DoctorName ?? "",
                    Status = ""
                })
                .ToListAsync(cancellationToken);

            var patient = await _patientGrpcClient.GetPatientAsync(request.PatientID, cancellationToken);

            if (patient == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_PATIENT_WITH_ID);
            }

            foreach(var exam in examinations)
            {
                var services = await _hospitalService.GetServicesByIdsAsync([exam.ServiceId], cancellationToken);
                var service = services.FirstOrDefault(s => s.Id == exam.ServiceId);

                exam.ServiceName = service!.ServiceName;
                exam.Status = exam.ReturnTime < DateTime.UtcNow ? "COMPLETED" : "PENDING";
            }

            var response = new GetAllExaminationHistoryOfPatientResponse 
            (
                PatientId : patient.Id,
                PatientCode : patient.Code,
                PatientName : patient.Name,
                DOB : patient.DOB,
                PhoneNumber: patient.PhoneNumber,
                ReturnDate:  examinations.FirstOrDefault()?.ReturnTime ?? DateTime.MinValue,
                ExaminationHistory: examinations
            );

            return response;
        }
    }
}
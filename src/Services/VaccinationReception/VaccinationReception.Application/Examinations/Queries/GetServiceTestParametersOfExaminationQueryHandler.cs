using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetServiceTestParametersOfExaminationQueryHandler : IQueryHandler<GetServiceTestParametersOfExaminationQuery, GetServiceTestParametersOfExaminationResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHospitalService _hospitalService;

        public GetServiceTestParametersOfExaminationQueryHandler(IApplicationDbContext context, IHospitalService hospitalServiceClient)
        {
            _context = context;
            _hospitalService = hospitalServiceClient;
        }

        public async Task<GetServiceTestParametersOfExaminationResponse> Handle(GetServiceTestParametersOfExaminationQuery request, CancellationToken cancellationToken)
        {
            if (request.ExaminationId <= 0)
            {
                throw new BadRequestException(ExceptionKey.INVALID_EXAMINATION_ID);
            }
            // Get the examination
            var examination = await _context.Examinations
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.ExaminationId, cancellationToken);

            if (examination == null || examination.ServiceId == null)
                return new GetServiceTestParametersOfExaminationResponse
                {
                    ServiceTestParameters = new List<ServiceTestParameterDTO>()
                };

            var services = await _hospitalService.GetServicesByIdsAsync([examination.ServiceId.Value], cancellationToken);

            var service = services.FirstOrDefault(s => s.Id == examination.ServiceId.Value);

            var serviceTestParameters = service!.ServiceTestParameters;

            var requestNumber = examination.RequestNumber ?? string.Empty;

            var response = new GetServiceTestParametersOfExaminationResponse
            {               
                ServiceTestParameters = serviceTestParameters!.Select(p => new ServiceTestParameterDTO
                {
                    RequestNumber = requestNumber,
                    ParameterName = p.ParameterName,
                    Result = "",
                    StandardValue = p.StandardValue,
                    Unit = p.Unit,
                    SpecimenType = p.SpecimenType,
                    EquipmentName = p.EquipmentName
                }).ToList()
            };

            return response;
        }
    }
}

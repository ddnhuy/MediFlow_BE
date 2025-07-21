using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetAllExaminationOfReceptionQueryHandler : IQueryHandler<GetAllExaminationOfReceptionQuery, GetAllExaminationOfReceptionQueryResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHospitalService _hospitalService;

        public GetAllExaminationOfReceptionQueryHandler(IApplicationDbContext context, IHospitalService hospitalService)
        {
            _context = context;
            _hospitalService = hospitalService;
        }

        public async Task<GetAllExaminationOfReceptionQueryResponse> Handle(GetAllExaminationOfReceptionQuery request, CancellationToken cancellationToken)
        {
            var examination = await _context.Examinations
                .Where(e => e.ReceptionId == request.ReceptionId)
                .ToListAsync(cancellationToken);

            var serviceIds = examination.Select(e => e.ServiceId!.Value).ToList();

            var services = await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken);

            var response = examination.Select(e => new GetAllExaminationOfReceptionItem(
                ExaminationId: e.Id,
                ServiceName: services.FirstOrDefault(s => s.Id == e.ServiceId)!.ServiceName ?? ""
            )).ToList();

            return new GetAllExaminationOfReceptionQueryResponse(response);
        }
    }
}

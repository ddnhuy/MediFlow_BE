using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId
{
    public class GetMedicineListForVaccinationByReceptionIdQueryHandler : IQueryHandler<GetMedicineListForVaccinationByReceptionIdQuery, List<GetMedicineListForVaccinationByReceptionIdResult>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IInventoryService _inventoryService;

        public GetMedicineListForVaccinationByReceptionIdQueryHandler(IApplicationDbContext dbContext, IInventoryService inventoryService)
        {
            _dbContext = dbContext;
            _inventoryService = inventoryService;
        }

        public async Task<List<GetMedicineListForVaccinationByReceptionIdResult>> Handle(GetMedicineListForVaccinationByReceptionIdQuery request, CancellationToken cancellationToken)
        {
            var receptionVaccinations = _dbContext.ReceptionVaccinations
                .Where(rv => rv.ReceptionId == request.ReceptionId).ToList();

            List<int> vaccineIdList = new List<int>();
            
            foreach(var receptionVaccination in receptionVaccinations)
            {
                vaccineIdList.Add(receptionVaccination.VaccineId);
            }

            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(vaccineIdList, cancellationToken);

            // Map to the result type
            var result = medicineInformationList.Select(m =>
                new GetMedicineListForVaccinationByReceptionIdResult(
                    m.MedicineId,
                    m.MedicineName!
                )
            ).ToList();

            return result;

        }
    }
}

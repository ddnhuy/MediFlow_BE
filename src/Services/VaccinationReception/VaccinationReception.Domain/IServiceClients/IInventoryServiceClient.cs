using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.DTOs;

namespace VaccinationReception.Domain.IServiceClients
{
    public interface IInventoryServiceClient
    {
        Task<MedicinePriceDTO?> GetMedicineByIdAsync(int medicineId, CancellationToken cancellationToken);
    }
}

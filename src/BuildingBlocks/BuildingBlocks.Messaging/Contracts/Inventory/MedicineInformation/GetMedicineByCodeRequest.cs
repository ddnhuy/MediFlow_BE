using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation
{
    public class GetMedicineByCodeRequest
    {
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public List<string> MedicineCodes { get; set; } = new();
    }
}

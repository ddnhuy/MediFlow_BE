using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ContractServiceDetail : BaseEntity
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public int? VaccineId { get; set; }
        public int? ServiceId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ActualQuantity { get; set; }
        public int? ActualTotalAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs
{
    public class ContractServiceDetailDTO
    {
        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public int? VaccineId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ActualQuantity { get; set; }
        public int? ActualTotalAmount { get; set; }
        public string? Name { get; set; }
    }
}

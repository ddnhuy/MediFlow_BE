using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class ServiceRequestDetailDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime InvoiceDate { get; set; }
        public PaymentStatusForItem PaymentStatus { get; set; }
        public string RequestNumber { get; set; } = null!;
        public string ServiceCode { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
    }
}

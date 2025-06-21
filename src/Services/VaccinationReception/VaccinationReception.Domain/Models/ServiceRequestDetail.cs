using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Domain.Models
{
    public class ServiceRequestDetail : BaseEntity
    {
        public int RequestFormId { get; set; }
        public RequestForm RequestForm { get; set; } = null!;

        public int ServiceId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime InvoiceDate { get; set; }
        public PaymentStatusForItem PaymentStatus { get; set; }
    }
}
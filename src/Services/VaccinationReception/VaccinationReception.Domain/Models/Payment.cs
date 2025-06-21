using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Domain.Models
{
    public class Payment : BaseEntity
    {
        public int ReceptionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Method { get; set; } = null!;
        public string? Note { get; set; }


        public string? ATMTransactionCode { get; set; }
        public PaymentType PaymentType { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? OfficialInvoiceNumber { get; set; }
        public PaymentStatus? Status { get; set; }

        public int? OriginalPaymentId { get; set; }
        public Payment? OriginalPayment { get; set; }

        // Navigation
        public Reception Reception { get; set; } = null!;
        public ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
    }
}

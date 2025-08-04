using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Domain.Models
{
    public class PaymentContract : BaseEntity
    {
        public int ContractId { get; set; }
        public string InvoiceNumber { get; set; }
        public string? VATInvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }

        public int CreatedByUserId { get; set; }

        public decimal TotalAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus? Status { get; set; }

        public string TaxCode { get; set; }
        public string OrganizationName { get; set; }

        public string? ATMCode { get; set; }
        // Navigation
        public Contract Contract { get; set; } = null!;
    }
}

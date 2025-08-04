using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs
{
    public class PaymentContractDTO
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string? VATInvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus? Status { get; set; }
        public string TaxCode { get; set; }
        public string OrganizationName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.DTOs.HospitalFeeDTOs
{
    public record PaymentDTO(
        int Id,
        int ReceptionId,
        decimal TotalAmount,
        PaymentMethod Method,
        string? Note,
        string? ATMTransactionCode,
        PaymentType PaymentType,
        string? InvoiceNumber,
        PaymentStatus? Status,
        int? OriginalPaymentId,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}

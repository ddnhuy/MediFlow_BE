using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class ReceptionVaccinationDTO
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int VaccineId { get; set; }
        public string? VaccineName { get; set; }
        public string? VaccineTypeName { get; set; }
        public bool IsConfirmed { get; set; }
        public string? Note { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Quantity { get; set; }
        public string? TestResultEntry { get; set; }
        public PaymentStatusForItem Status { get; set; }
        public int ReceptionId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool IsReadyToUse { get; set; }
        public int DoctorId { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
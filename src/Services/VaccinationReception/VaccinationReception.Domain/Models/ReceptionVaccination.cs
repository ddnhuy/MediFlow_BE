using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ReceptionVaccination : BaseEntity
    {
        public int ReceptionId { get; set; }
        public int VaccineId { get; set; }
        public int Quantity { get; set; }
        public bool IsReadyToUse { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsConfirmed { get; set; }
        public string? Note { get; set; }
        public string? TestResultEntry { get; set; }
        public int DoctorId { get; set; }

        // Navigation
        public Reception Reception { get; set; } = null!;
    }
}
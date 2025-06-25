using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class PaymentDetail : BaseEntity
    {
        public int PaymentId { get; set; }

        public int? ReceptionVaccinationId { get; set; }
        public int? ServiceRequestDetailId { get; set; }

        public decimal Amount { get; set; }
        public bool IsReversed => Amount < 0;

        // Navigation
        public Payment Payment { get; set; } = null!;
        public ReceptionVaccination? ReceptionVaccination { get; set; }
        public ServiceRequestDetail? ServiceRequestDetail { get; set; }
    }
}

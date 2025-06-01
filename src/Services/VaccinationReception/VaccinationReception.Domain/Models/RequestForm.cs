using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class RequestForm : BaseEntity
    {
        public int ReceptionId { get; set; }
        public Reception Reception { get; set; } = null!;
        public string RequestNumber { get; set; } = null!;

        public ICollection<ServiceRequestDetail> ServiceRequestDetails { get; set; } = new List<ServiceRequestDetail>();
    }
}
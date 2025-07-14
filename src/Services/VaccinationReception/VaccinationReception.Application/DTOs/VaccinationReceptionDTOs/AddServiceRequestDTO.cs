using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class AddServiceRequestDTO
    {
        public int ReceptionId { get; set; }
        public List<ServiceRequestItemDTO>? Services { get; set; }
        public string? GroupType { get; set; }
        public int? GroupId { get; set; }
        public int DefaultQuantity { get; set; } = 1;
    }
    public class ServiceRequestItemDTO
    {
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
    }
}
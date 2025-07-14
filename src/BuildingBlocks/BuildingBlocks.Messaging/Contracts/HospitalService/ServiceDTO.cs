using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.HospitalService
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceName { get; set; }
        public decimal UnitPrice { get; set; }
        public int DepartmentId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.HospitalService.GetServiceByServiceCodes
{
    public class GetServiceByServiceCode
    {
        public List<string> ServiceCodes { get; set; }
    }

    public class GetServicesByServiceCodeResponse
    {
        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}

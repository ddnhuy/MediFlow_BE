using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup
{
    public class GetServicesByGroupResponse
    {
        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}

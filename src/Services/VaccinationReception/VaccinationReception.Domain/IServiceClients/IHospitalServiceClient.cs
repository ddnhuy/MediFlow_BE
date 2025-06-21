using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.DTOs;

namespace VaccinationReception.Domain.IServiceClients
{
    public interface IHospitalServiceClient
    {
        Task<List<ServiceResponse>> GetServicesByGroupAsync(int groupId, string groupType, CancellationToken cancellationToken);
        Task<List<ServiceResponse>> GetServicesByIdsAsync(List<int> serviceIds, CancellationToken cancellationToken);
    }

}

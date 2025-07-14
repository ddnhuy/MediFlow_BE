using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Repositories
{
    public interface IServiceGroupServiceRepository
    {
        Task<IEnumerable<ServiceGroupService>> GetByServiceGroupIdAsync(int serviceGroupId);
        Task AddRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices);
        Task UpdateRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices);
        Task<IEnumerable<int>> GetExistingServiceIdsAsync(int serviceGroupId);
        Task DeleteRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices);
        Task<ServiceGroupService> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int serviceGroupId, int serviceId);
        Task<IEnumerable<ServiceGroupService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
    }
}

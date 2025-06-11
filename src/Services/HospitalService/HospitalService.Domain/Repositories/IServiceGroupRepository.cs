using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Repositories
{
    public interface IServiceGroupRepository
    {
        Task<ServiceGroup> GetByIdAsync(int id);
        Task<IEnumerable<ServiceGroup>> GetAllAsync();
        Task<ServiceGroup> AddAsync(ServiceGroup serviceGroup);
        Task<ServiceGroup> UpdateAsync(ServiceGroup serviceGroup);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ServiceGroup>> GetBySearchTermAsync(string searchTerm);
        Task<int> GetTotalCountAsync(string searchTerm);
        Task<ServiceGroup> GetByIdWithServicesAsync(int id);
        Task<IEnumerable<ServiceGroup>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default);
        Task<(IEnumerable<ServiceGroup> Items, int TotalCount)> GetPaginatedAsync(int pageIndex, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
    }
}

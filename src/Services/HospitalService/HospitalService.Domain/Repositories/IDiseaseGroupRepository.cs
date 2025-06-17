using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Repositories
{
    public interface IDiseaseGroupRepository
    {
        Task<DiseaseGroup> GetByIdAsync(int id);
        Task<IEnumerable<DiseaseGroup>> GetAllAsync();
        Task<DiseaseGroup> AddAsync(DiseaseGroup serviceGroup);
        Task<DiseaseGroup> UpdateAsync(DiseaseGroup serviceGroup);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<DiseaseGroup>> GetBySearchTermAsync(string searchTerm);
        Task<int> GetTotalCountAsync(string searchTerm);
        Task<DiseaseGroup> GetByIdWithServicesAsync(int id);
        Task<IEnumerable<DiseaseGroup>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default);
        Task<(IEnumerable<DiseaseGroup> Items, int TotalCount)> GetPaginatedAsync(int pageIndex, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
    }
}

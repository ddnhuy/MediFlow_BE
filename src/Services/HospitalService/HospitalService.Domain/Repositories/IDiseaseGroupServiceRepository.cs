using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Repositories
{
    public interface IDiseaseGroupServiceRepository
    {
        Task<IEnumerable<DiseaseGroupService>> GetByServiceGroupIdAsync(int diseaseGroupId);
        Task AddRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices);
        Task UpdateRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices);
        Task<IEnumerable<int>> GetExistingServiceIdsAsync(int diseaseGroupId);
        Task DeleteRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices);
        Task<DiseaseGroupService> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int diseaseGroupId, int serviceId);
        Task<IEnumerable<DiseaseGroupService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
    }
}

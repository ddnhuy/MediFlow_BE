using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Repositories
{
    public interface IServiceRepository
    {
        Task<Service> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken);
        Task<Service> AddAsync(Service service, CancellationToken cancellationToken);
        Task<Service> UpdateAsync(Service service, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetBySearchTermAsync(string searchTerm, CancellationToken cancellationToken);
        Task<int> GetTotalCountAsync(string searchTerm, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetServicesByGroupIdAsync(int groupId, string groupType, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetByIdsAsync(List<int> serviceIds, CancellationToken cancellationToken);
        Task<List<Service>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Service>> GetByServiceCodesAsync(List<string> serviceCodes, CancellationToken cancellationToken);
    }
}

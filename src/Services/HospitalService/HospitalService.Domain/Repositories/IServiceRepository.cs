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
        Task<Service> GetByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service> AddAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Service>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<Service>> GetBySearchTermAsync(string searchTerm);
        Task<int> GetTotalCountAsync(string searchTerm);
    }
}

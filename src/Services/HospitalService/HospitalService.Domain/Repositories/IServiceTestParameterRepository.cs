using HospitalService.Domain.Models;

namespace HospitalService.Domain.Repositories
{
    public interface IServiceTestParameterRepository
    {
        Task AddRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceTestParameter>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default);
    }
}

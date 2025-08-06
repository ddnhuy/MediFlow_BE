using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using HospitalService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Repositories
{
    public class ServiceTestParameterRepository : IServiceTestParameterRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceTestParameterRepository> _logger;

        public ServiceTestParameterRepository(ApplicationDbContext context, ILogger<ServiceTestParameterRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default)
        {
            await _context.ServiceTestParameters.AddRangeAsync(parameters, cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default)
        {
            // First, delete existing parameters for the service
            var serviceId = parameters.FirstOrDefault()?.ServiceId;
            if (serviceId.HasValue)
            {
                var existingParameters = await _context.ServiceTestParameters
                    .Where(p => p.ServiceId == serviceId.Value)
                    .ToListAsync(cancellationToken);

                _context.ServiceTestParameters.RemoveRange(existingParameters);
            }

            // Then add new parameters
            await _context.ServiceTestParameters.AddRangeAsync(parameters, cancellationToken);
            _logger.LogInformation("Updated {Count} service test parameters for service {ServiceId}", parameters.Count(), serviceId);
        }

        public async Task<IEnumerable<ServiceTestParameter>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default)
        {
            return await _context.ServiceTestParameters
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<ServiceTestParameter> parameters, CancellationToken cancellationToken = default)
        {
            foreach (var parameter in parameters)
            {
                parameter.IsCancelled = true;
            }
            _context.ServiceTestParameters.UpdateRange(parameters);
            _logger.LogInformation("Deleted {Count} service test parameters", parameters.Count());
            await Task.CompletedTask;
        }
    }
}
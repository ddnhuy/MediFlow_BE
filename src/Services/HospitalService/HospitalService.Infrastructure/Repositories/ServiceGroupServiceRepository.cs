using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using HospitalService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Repositories
{
    public class ServiceGroupServiceRepository : IServiceGroupServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceGroupServiceRepository> _logger;

        public ServiceGroupServiceRepository(ApplicationDbContext context, ILogger<ServiceGroupServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiceGroupService>> GetByServiceGroupIdAsync(int serviceGroupId)
        {
            return await _context.ServiceGroupServices
                .Include(sgs => sgs.Service)
                .Where(sgs => sgs.ServiceGroupId == serviceGroupId && !sgs.IsCancelled)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices)
        {
            await _context.ServiceGroupServices.AddRangeAsync(serviceGroupServices);
        }

        public async Task UpdateRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices)
        {
            _context.ServiceGroupServices.UpdateRange(serviceGroupServices);
        }

        public async Task<IEnumerable<int>> GetExistingServiceIdsAsync(int serviceGroupId)
        {
            return await _context.ServiceGroupServices
                .Where(sgs => sgs.ServiceGroupId == serviceGroupId && !sgs.IsCancelled)
                .Select(sgs => sgs.ServiceId)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<ServiceGroupService> serviceGroupServices)
        {
            foreach (var serviceGroupService in serviceGroupServices)
            {
                serviceGroupService.IsCancelled = true;
            }
            _context.ServiceGroupServices.UpdateRange(serviceGroupServices);
        }

        public async Task<ServiceGroupService> GetByIdAsync(int id)
        {
            return await _context.ServiceGroupServices
                .Include(sgs => sgs.Service)
                .FirstOrDefaultAsync(sgs => sgs.Id == id);
        }

        public async Task<bool> ExistsAsync(int serviceGroupId, int serviceId)
        {
            return await _context.ServiceGroupServices
                .AnyAsync(sgs => sgs.ServiceGroupId == serviceGroupId &&
                                sgs.ServiceId == serviceId &&
                                !sgs.IsCancelled);
        }
    }
}

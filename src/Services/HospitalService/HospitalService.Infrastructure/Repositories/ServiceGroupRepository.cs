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
    public class ServiceGroupRepository : IServiceGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceGroupRepository> _logger;

        public ServiceGroupRepository(ApplicationDbContext context, ILogger<ServiceGroupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceGroup> GetByIdAsync(int id)
        {
            return await _context.ServiceGroups
                .FirstOrDefaultAsync(sg => sg.Id == id);
        }

        public async Task<ServiceGroup> GetByIdWithServicesAsync(int id)
        {
            return await _context.ServiceGroups
                .Include(sg => sg.ServiceGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .FirstOrDefaultAsync(sg => sg.Id == id);
        }

        public async Task<IEnumerable<ServiceGroup>> GetAllAsync()
        {
            return await _context.ServiceGroups
                .Include(sg => sg.ServiceGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .ToListAsync();
        }

        public async Task<ServiceGroup> AddAsync(ServiceGroup serviceGroup)
        {
            await _context.ServiceGroups.AddAsync(serviceGroup);
            return serviceGroup;
        }

        public async Task<ServiceGroup> UpdateAsync(ServiceGroup serviceGroup)
        {
            _context.ServiceGroups.Update(serviceGroup);
            return serviceGroup;
        }

        public async Task DeleteAsync(int id)
        {
            var serviceGroup = await GetByIdAsync(id);
            if (serviceGroup != null)
            {
                serviceGroup.IsCancelled = true;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceGroups.AnyAsync(sg => sg.Id == id);
        }

        public async Task<IEnumerable<ServiceGroup>> GetBySearchTermAsync(string searchTerm)
        {
            return await _context.ServiceGroups
                .Where(sg => string.IsNullOrEmpty(searchTerm) ||
                           sg.GroupName.ToLower().Contains(searchTerm.ToLower()))
                .Include(sg => sg.ServiceGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string searchTerm)
        {
            return await _context.ServiceGroups
                .Where(sg => string.IsNullOrEmpty(searchTerm) ||
                           sg.GroupName.ToLower().Contains(searchTerm.ToLower()))
                .CountAsync();
        }

        public async Task<IEnumerable<ServiceGroup>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = _context.ServiceGroups.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(sg =>
                    sg.GroupName.ToLower().Contains(searchTerm.ToLower()));
            }

            return await query
                .OrderByDescending(sg => sg.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<ServiceGroup> Items, int TotalCount)> GetPaginatedAsync(
            int pageIndex,
            int pageSize,
            string? searchTerm,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ServiceGroups.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(sg =>
                    sg.GroupName.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(sg => sg.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}

using BuildingBlocks.Strings;
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
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(ApplicationDbContext context, ILogger<ServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Service> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Services
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Services
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync(cancellationToken);
        }

        public async Task<Service> AddAsync(Service service, CancellationToken cancellationToken)
        {
            await _context.Services.AddAsync(service, cancellationToken);
            return service;
        }

        public async Task<Service> UpdateAsync(Service service, CancellationToken cancellationToken)
        {
            _context.Services.Update(service);
            return service;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var service = await GetByIdAsync(id, cancellationToken);
            if (service != null)
            {
                service.IsCancelled = true;
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Services.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken)
        {
            return await _context.Services
                .Where(s => s.DepartmentId == departmentId)
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Service>> GetBySearchTermAsync(string searchTerm, CancellationToken cancellationToken)
        {
            return await _context.Services
                .Where(s => s.ServiceName.ToLower().Contains(searchTerm.ToLower()) ||
                           s.ServiceCode.ToLower().Contains(searchTerm.ToLower()))
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(string searchTerm, CancellationToken cancellationToken)
        {
            return await _context.Services
                .Where(s => string.IsNullOrEmpty(searchTerm) ||
                           s.ServiceName.ToLower().Contains(searchTerm.ToLower()) ||
                           s.ServiceCode.ToLower().Contains(searchTerm.ToLower()))
                .CountAsync(cancellationToken);
        }
        public async Task<IEnumerable<Service>> GetServicesByGroupIdAsync(int groupId, string groupType, CancellationToken cancellationToken)
        {
            IQueryable<Service> query = _context.Services;

            var normalizedGroupType = groupType?.Trim();

            if (string.Equals(normalizedGroupType, GroupServiceType.SERVICE_GROUP, StringComparison.OrdinalIgnoreCase))
            {
                var serviceIds = await _context.ServiceGroupServices
                    .Where(sgs => sgs.ServiceGroupId == groupId)
                    .Select(sgs => sgs.ServiceId)
                    .ToListAsync(cancellationToken);

                query = query.Where(s => serviceIds.Contains(s.Id));
            }
            else if (string.Equals(normalizedGroupType, GroupServiceType.DISEASE_GROUP, StringComparison.OrdinalIgnoreCase))
            {
                var serviceIds = await _context.DiseaseGroupServices
                    .Where(dgs => dgs.DiseaseGroupId == groupId)
                    .Select(dgs => dgs.ServiceId)
                    .ToListAsync(cancellationToken);

                query = query.Where(s => serviceIds.Contains(s.Id));
            }

            return await query
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Service>> GetByIdsAsync(List<int> serviceIds, CancellationToken cancellationToken)
        {
            if (serviceIds == null || !serviceIds.Any())
            {
                return new List<Service>();
            }

            return await _context.Services
                .Where(s => serviceIds.Contains(s.Id) && !s.IsCancelled)
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Service>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Services
                .Where(s => !s.IsCancelled)
                .Include(s => s.ServiceGroupServices)
                    .ThenInclude(sgs => sgs.ServiceGroup)
                .Include(s => s.DiseaseGroupServices)
                    .ThenInclude(dgs => dgs.DiseaseGroup)
                .ToListAsync(cancellationToken);
        }
    }
}

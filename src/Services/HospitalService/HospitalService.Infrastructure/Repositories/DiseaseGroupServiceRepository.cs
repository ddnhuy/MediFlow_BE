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
    public class DiseaseGroupServiceRepository : IDiseaseGroupServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiseaseGroupServiceRepository> _logger;

        public DiseaseGroupServiceRepository(ApplicationDbContext context, ILogger<DiseaseGroupServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DiseaseGroupService>> GetByServiceGroupIdAsync(int diseaseGroupId)
        {
            return await _context.DiseaseGroupServices
                .Include(sgs => sgs.Service)
                .Where(sgs => sgs.DiseaseGroupId == diseaseGroupId && !sgs.IsCancelled)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices)
        {
            await _context.DiseaseGroupServices.AddRangeAsync(diseaseGroupServices);
        }

        public async Task UpdateRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices)
        {
            _context.DiseaseGroupServices.UpdateRange(diseaseGroupServices);
        }

        public async Task<IEnumerable<int>> GetExistingServiceIdsAsync(int diseaseGroupId)
        {
            return await _context.DiseaseGroupServices
                .Where(sgs => sgs.DiseaseGroupId == diseaseGroupId && !sgs.IsCancelled)
                .Select(sgs => sgs.ServiceId)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<DiseaseGroupService> diseaseGroupServices)
        {
            foreach (var diseaseGroupService in diseaseGroupServices)
            {
                diseaseGroupService.IsCancelled = true;
            }
            _context.DiseaseGroupServices.UpdateRange(diseaseGroupServices);
        }

        public async Task<DiseaseGroupService> GetByIdAsync(int id)
        {
            return await _context.DiseaseGroupServices
                .Include(sgs => sgs.Service)
                .FirstOrDefaultAsync(sgs => sgs.Id == id);
        }

        public async Task<bool> ExistsAsync(int diseaseGroupId, int serviceId)
        {
            return await _context.DiseaseGroupServices
                .AnyAsync(sgs => sgs.DiseaseGroupId == diseaseGroupId &&
                                sgs.ServiceId == serviceId &&
                                !sgs.IsCancelled);
        }
        public async Task<IEnumerable<DiseaseGroupService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default)
        {
            return await _context.DiseaseGroupServices
                .Where(dgs => dgs.ServiceId == serviceId)
                .ToListAsync(cancellationToken);
        }
    }
}

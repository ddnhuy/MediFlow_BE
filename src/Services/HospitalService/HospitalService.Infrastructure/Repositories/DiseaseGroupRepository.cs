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
    public class DiseaseGroupRepository : IDiseaseGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiseaseGroupRepository> _logger;

        public DiseaseGroupRepository(ApplicationDbContext context, ILogger<DiseaseGroupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DiseaseGroup> GetByIdAsync(int id)
        {
            return await _context.DiseaseGroups
                .FirstOrDefaultAsync(sg => sg.Id == id);
        }

        public async Task<DiseaseGroup> GetByIdWithServicesAsync(int id)
        {
            return await _context.DiseaseGroups
                .Include(sg => sg.DiseaseGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .FirstOrDefaultAsync(sg => sg.Id == id);
        }

        public async Task<IEnumerable<DiseaseGroup>> GetAllAsync()
        {
            return await _context.DiseaseGroups
                .Include(sg => sg.DiseaseGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .ToListAsync();
        }

        public async Task<DiseaseGroup> AddAsync(DiseaseGroup diseaseGroup)
        {
            await _context.DiseaseGroups.AddAsync(diseaseGroup);
            return diseaseGroup;
        }

        public async Task<DiseaseGroup> UpdateAsync(DiseaseGroup diseaseGroup)
        {
            _context.DiseaseGroups.Update(diseaseGroup);
            return diseaseGroup;
        }

        public async Task DeleteAsync(int id)
        {
            var diseaseGroup = await GetByIdAsync(id);
            if (diseaseGroup != null)
            {
                diseaseGroup.IsCancelled = true;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.DiseaseGroups.AnyAsync(sg => sg.Id == id);
        }

        public async Task<IEnumerable<DiseaseGroup>> GetBySearchTermAsync(string searchTerm)
        {
            return await _context.DiseaseGroups
                .Where(sg => string.IsNullOrEmpty(searchTerm) ||
                           sg.GroupName.ToLower().Contains(searchTerm.ToLower()))
                .Include(sg => sg.DiseaseGroupServices)
                    .ThenInclude(sgs => sgs.Service)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string searchTerm)
        {
            return await _context.DiseaseGroups
                .Where(sg => string.IsNullOrEmpty(searchTerm) ||
                           sg.GroupName.ToLower().Contains(searchTerm.ToLower()))
                .CountAsync();
        }

        public async Task<IEnumerable<DiseaseGroup>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = _context.DiseaseGroups.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(sg =>
                    sg.GroupName.ToLower().Contains(searchTerm.ToLower()));
            }

            return await query
                .OrderByDescending(sg => sg.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<DiseaseGroup> Items, int TotalCount)> GetPaginatedAsync(
            int pageIndex,
            int pageSize,
            string? searchTerm,
            CancellationToken cancellationToken = default)
        {
            var query = _context.DiseaseGroups.AsQueryable();

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

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

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync();
        }

        public async Task<Service> AddAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            return service;
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            return service;
        }

        public async Task DeleteAsync(int id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                service.IsCancelled = true;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Services.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Service>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Services
                .Where(s => s.DepartmentId == departmentId)
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetBySearchTermAsync(string searchTerm)
        {
            return await _context.Services
                .Where(s => s.ServiceName.ToLower().Contains(searchTerm.ToLower()) ||
                           s.ServiceCode.ToLower().Contains(searchTerm.ToLower()))
                .Include(s => s.ServiceGroupServices)
                .Include(s => s.DiseaseGroupServices)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string searchTerm)
        {
            return await _context.Services
                .Where(s => string.IsNullOrEmpty(searchTerm) ||
                           s.ServiceName.ToLower().Contains(searchTerm.ToLower()) ||
                           s.ServiceCode.ToLower().Contains(searchTerm.ToLower()))
                .CountAsync();
        }
    }
}

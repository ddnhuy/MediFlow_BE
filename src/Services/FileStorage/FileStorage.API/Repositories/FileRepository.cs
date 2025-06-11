using BuildingBlocks.Strings.Enums;
using FileStorage.API.Database;
using FileStorage.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.API.Repositories
{
    public interface IFileRepository
    {
        Task<FileMetadata?> GetByIdAsync(Guid id);
        Task<List<FileMetadata>> GetByFilterAsync(string? department, FileType? type);
        Task CreateAsync(FileMetadata file);
        Task DeleteAsync(FileMetadata fileMetadata);
    }

    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;

        public FileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FileMetadata?> GetByIdAsync(Guid id)
        {
            return await _context.FileMetadatas.FindAsync(id);
        }

        public async Task<List<FileMetadata>> GetByFilterAsync(string? department, FileType? type)
        {
            var query = _context.FileMetadatas.AsQueryable();

            if (!string.IsNullOrEmpty(department))
                query = query.Where(f => f.Department.ToLower().Contains(department.ToLower()));

            if (type.HasValue)
                query = query.Where(f => f.Type == type);

            return await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }

        public async Task CreateAsync(FileMetadata file)
        {
            _context.FileMetadatas.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FileMetadata fileMetadata)
        {
            _context.FileMetadatas.Remove(fileMetadata);
            await _context.SaveChangesAsync();
        }
    }
}
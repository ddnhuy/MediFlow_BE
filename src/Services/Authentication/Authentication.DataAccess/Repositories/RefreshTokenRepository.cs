using Authentication.DataAccess.Data;
using Authentication.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DataAccess.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(string token, int userId);
        Task<int> FindAsync(string token);
        Task DeleteAsync(string token, int userId);
        Task DeleteAllForUserAsync(int userId);
    }

    public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
    {
        public async Task AddAsync(string token, int userId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = userId,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            };

            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task<int> FindAsync(string token)
        {
            var refreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                return -1;
            }

            return refreshToken.UserId;
        }

        public async Task DeleteAsync(string token, int userId)
        {
            var existingToken = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.UserId == userId);

            if (existingToken != null)
            {
                dbContext.RefreshTokens.Remove(existingToken);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllForUserAsync(int userId)
        {
            var tokens = await dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();

            dbContext.RefreshTokens.RemoveRange(tokens);
            await dbContext.SaveChangesAsync();
        }
    }
}

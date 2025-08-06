using Authentication.DataAccess.Data;
using Authentication.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DataAccess.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(string token, int userId, string roles);
        Task<(int UserId, string Roles)> FindAsync(string token);
        Task DeleteAllForUserAsync(int userId, string currentToken);
    }

    public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
    {
        public async Task AddAsync(string token, int userId, string roles)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = userId,
                Roles = roles,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            };

            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task<(int UserId, string Roles)> FindAsync(string token)
        {
            var refreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                return (-1, string.Empty);
            }

            return (refreshToken.UserId, refreshToken.Roles);
        }

        public async Task DeleteAllForUserAsync(int userId, string currentToken)
        {
            var tokens = await dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.Token != currentToken)
                .ToListAsync();

            dbContext.RefreshTokens.RemoveRange(tokens);
            await dbContext.SaveChangesAsync();
        }
    }
}

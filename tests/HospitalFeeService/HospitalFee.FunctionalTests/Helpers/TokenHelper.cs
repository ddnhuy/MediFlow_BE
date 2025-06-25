using BuildingBlocks.Strings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace HospitalFee.FunctionalTests.Helpers
{
    public class TokenHelper
    {
        public static string GenerateTestToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, Roles.ADMIN),
                new Claim("department", "testing")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("super-duper-secret-value-that-should-be-in-user-secrets"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MediFlowIssuer",
                audience: "MediFlowAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

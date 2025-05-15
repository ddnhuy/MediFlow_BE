using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record RevokeRefreshTokenRequest(string CurrentRefreshToken);
    public record RevokeRefreshTokenResponse(bool IsSuccess, string Message);

    public class RevokeRefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/authentication/revoke-refresh-token", [Authorize] async (RevokeRefreshTokenRequest request, ISender sender, HttpContext context) =>
            {
                int.TryParse(
                    context.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value,
                    out int userId);

                var result = await sender.Send(new RevokeRefreshTokenCommand(userId, request.CurrentRefreshToken));

                var response = result.Adapt<RevokeRefreshTokenResponse>();

                return Results.Ok(response);
            })
            .WithName("RevokeRefreshToken")
            .Produces<RevokeRefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Revoke Refresh Token")
            .WithDescription("Revoke Refresh Token");
        }
    }
}

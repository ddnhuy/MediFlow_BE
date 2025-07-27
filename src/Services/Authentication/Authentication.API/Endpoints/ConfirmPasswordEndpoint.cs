using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record ConfirmPasswordRequest(string Password);
    public record ConfirmPasswordResponse(bool IsSuccess, string Message);

    public class ConfirmPasswordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/confirm-password", [Authorize] async (ConfirmPasswordRequest request, ISender sender, HttpContext context) =>
            {
                // Extract user ID from JWT token
                var userId = int.Parse(context.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

                var result = await sender.Send(new ConfirmPasswordCommand(userId, request.Password));

                var response = result.Adapt<ConfirmPasswordResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ConfirmPassword")
            .Produces<ConfirmPasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Confirm Password")
            .WithDescription("Verify the current user's password for security confirmation.");
        }
    }
}
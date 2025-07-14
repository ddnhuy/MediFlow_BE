using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record ChangePasswordRequest(int UserId, string CurrentPassword, string NewPassword);
    public record ChangePasswordResponse(bool IsSuccess, string Message);

    public class ChangePasswordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/change-password", [Authorize] async (ChangePasswordRequest request, ISender sender, HttpContext context) =>
            {
                var result = await sender.Send(
                    new ChangePasswordCommand(
                        request.UserId,
                        request.CurrentPassword,
                        request.NewPassword,
                        context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value));

                var response = result.Adapt<ChangePasswordResponse>();

                return Results.Ok(response);
            })
            .WithName("ChangePassword")
            .Produces<ChangePasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Change Password")
            .WithDescription("Change Password");
        }
    }
}

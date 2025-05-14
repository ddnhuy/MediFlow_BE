using Microsoft.AspNetCore.Authorization;

namespace Authentication.API.Endpoints
{
    public record ChangePasswordRequest(int UserId, string CurrentPassword, string NewPassword);
    public record ChangePasswordResponse(bool IsSuccess, string Message);

    public class ChangePasswordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/authentication/change-password", [Authorize] async (ChangePasswordRequest request, ISender sender) =>
            {
                var command = request.Adapt<ChangePasswordCommand>();

                var result = await sender.Send(command);

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

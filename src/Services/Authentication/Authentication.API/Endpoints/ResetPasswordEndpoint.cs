namespace Authentication.API.Endpoints
{
    public record ResetPasswordRequest(string Email);
    public record ResetPasswordResponse(bool IsSuccess, string Message);

    public class ResetPasswordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/authentication/reset-password", async (ResetPasswordRequest request, ISender sender) =>
            {
                var command = request.Adapt<ResetPasswordCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<ResetPasswordResponse>();

                return Results.Ok(response);
            })
            .WithName("ResetPassword")
            .Produces<ResetPasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reset Password")
            .WithDescription("Reset Password");
        }
    }
}

namespace Authentication.API.Endpoints
{
    public record LoginWithRefreshTokenRequest(string RefreshToken);
    public record LoginWithRefreshTokenResponse(string AccessToken, string RefreshToken);

    public class LoginWithRefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/refresh-token", async (LoginWithRefreshTokenRequest request, ISender sender) =>
            {
                var command = request.Adapt<LoginWithRefreshTokenCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<LoginWithRefreshTokenResponse>();

                return Results.Ok(response);
            })
            .WithName("LoginWithRefreshToken")
            .Produces<LoginWithRefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login With Refresh Token")
            .WithDescription("Login With Refresh Token");
        }
    }
}

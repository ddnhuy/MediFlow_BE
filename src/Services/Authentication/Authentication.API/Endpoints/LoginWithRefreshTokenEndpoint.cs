using Authentication.API.Helpers;

namespace Authentication.API.Endpoints
{
    public record LoginWithRefreshTokenRequest(string RefreshToken);
    public record LoginWithRefreshTokenResponse(bool IsSuccess, string Message);

    public class LoginWithRefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/refresh-token", async (LoginWithRefreshTokenRequest request, ISender sender, HttpContext context) =>
            {
                var command = request.Adapt<LoginWithRefreshTokenCommand>();

                var result = await sender.Send(command);

                HttpCookiesHelper.AppendAuthCookies(context.Response, result.AccessToken, result.RefreshToken);

                return Results.Ok(new LoginWithUserNameResponse(true, "Đăng nhập thành công."));
            })
            .WithName("LoginWithRefreshToken")
            .Produces<LoginWithRefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login With Refresh Token")
            .WithDescription("Login With Refresh Token");
        }
    }
}

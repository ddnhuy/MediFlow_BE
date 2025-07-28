using Authentication.API.Helpers;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record LoginWithRefreshTokenResponse(bool IsSuccess, string Message);

    public class LoginWithRefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/refresh-token", async (ISender sender, HttpContext context) =>
            {
                var refreshToken = context.Request.Cookies["refresh_token"]!;
                var roles = context.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                var command = new LoginWithRefreshTokenCommand(refreshToken, roles);

                var result = await sender.Send(command);

                HttpCookiesHelper.AppendAuthCookies(context.Response, result.AccessToken, result.RefreshToken);

                return Results.Ok(new LoginWithUserNameResponse(true, "LOGIN_SUCCESSFUL"));
            })
            .WithName("LoginWithRefreshToken")
            .Produces<LoginWithRefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login With Refresh Token")
            .WithDescription("Login With Refresh Token");
        }
    }
}

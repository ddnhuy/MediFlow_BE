namespace Authentication.API.Endpoints
{
    public record LoginWithRefreshTokenRequest(string RefreshToken);
    public record LoginWithRefreshTokenResponse(bool IsSuccess, string Message);

    public class LoginWithRefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/refresh-token", async (LoginWithRefreshTokenRequest request, ISender sender, HttpResponse httpResponse) =>
            {
                var command = request.Adapt<LoginWithRefreshTokenCommand>();

                var result = await sender.Send(command);

                // Send cookies HttpOnly
                httpResponse.Cookies.Append("access_token", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });
                httpResponse.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

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

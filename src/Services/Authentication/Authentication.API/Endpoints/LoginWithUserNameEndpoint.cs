namespace Authentication.API.Endpoints
{
    public record LoginWithUserNameRequest(string UserName, string Password);
    public record LoginWithUserNameResponse(bool IsSuccess, string Message);

    public class LoginWithUserNameEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (LoginWithUserNameRequest request, ISender sender, HttpResponse httpResponse) =>
            {
                var command = request.Adapt<LoginWithUserNameCommand>();

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
            .WithName("LoginWithUserName")
            .Produces<LoginWithUserNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login With UserName")
            .WithDescription("Login With UserName");
        }
    }
}

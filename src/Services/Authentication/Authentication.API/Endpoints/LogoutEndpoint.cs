namespace Authentication.API.Endpoints
{
    public record LogoutResponse(bool IsSuccess, string Message);

    public class LogoutEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/logout", (ISender sender, HttpContext context) =>
            {
                context.Response.Cookies.Append("access_token", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    SameSite = SameSiteMode.None
                });

                context.Response.Cookies.Append("refresh_token", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    SameSite = SameSiteMode.None
                });

                return Results.Ok(new LogoutResponse(true, "Đăng xuất thành công."));
            })
            .WithName("Logout")
            .Produces<LogoutResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Logout")
            .WithDescription("Logout");
        }
    }
}

using Authentication.API.Helpers;

namespace Authentication.API.Endpoints
{
    public record LoginWithUserNameRequest(string UserName, string Password);
    public record LoginWithUserNameResponse(bool IsSuccess, string Message);

    public class LoginWithUserNameEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (LoginWithUserNameRequest request, ISender sender, HttpContext context) =>
            {
                var command = request.Adapt<LoginWithUserNameCommand>();

                var result = await sender.Send(command);

                HttpCookiesHelper.AppendAuthCookies(context.Response, result.AccessToken, result.RefreshToken);

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

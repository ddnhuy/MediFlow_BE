namespace Authentication.API.Endpoints
{
    public record LoginWithUserNameRequest(string UserName, string Password);
    public record LoginWithUserNameResponse(string AccessToken, string RefreshToken);

    public class LoginWithUserNameEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/authentication/login", async (LoginWithUserNameRequest request, ISender sender) =>
            {
                var command = request.Adapt<LoginWithUserNameCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<LoginWithUserNameResponse>();

                return Results.Ok(response);
            })
            .WithName("LoginWithUserName")
            .Produces<LoginWithUserNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login With UserName")
            .WithDescription("Login With UserName");
        }
    }
}

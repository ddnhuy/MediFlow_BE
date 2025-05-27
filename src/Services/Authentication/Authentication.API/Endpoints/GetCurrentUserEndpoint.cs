using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record GetCurrentUserResponse(int Id, string Code, string UserName, string Email, string Name, string? ProfilePictureUrl, string Roles);

    public class GetCurrentUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/current-user", [Authorize] async (ISender sender, HttpContext context) =>
            {
                var userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

                var result = await sender.Send(new GetCurrentUserQuery(int.Parse(userId)));

                return Results.Ok(result.Adapt<GetCurrentUserResponse>());
            })
            .WithName("GetCurrentUser")
            .Produces<GetCurrentUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Current User")
            .WithDescription("Get Current User");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authentication.API.Endpoints
{
    public record GetCurrentUserPoliciesResponse(IEnumerable<string> Roles, IEnumerable<string> Departments, IDictionary<string, string> ResourceTypes);

    public class GetCurrentUserPoliciesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/current-user/policies", [Authorize] async (ISender sender, HttpContext context) =>
            {
                var roles = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)!.Value;
                var departments = context.User.Claims.FirstOrDefault(x => x.Type == "department")!.Value;

                var result = await sender.Send(new GetCurrentUserPoliciesQuery(roles, departments.Replace('_', ' ')));

                return Results.Ok(result.Adapt<GetCurrentUserPoliciesResponse>());
            })
            .WithName("GetCurrentUserPolicies")
            .Produces<GetCurrentUserPoliciesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Current User Policies")
            .WithDescription("Get Current User Policies");
        }
    }
}

using Inventory.Application.UniqueCode.Queries;

namespace Inventory.API.Endpoints
{
    public record GenerateUniqueCodeResponse(string UniqueCode);
    public class GenerateUniqueCodeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/unique-code/generate", async (ISender sender) =>
            {
                var result = await sender.Send(new GenerateUniqueCodeQuery());

                var response = result.Adapt<GenerateUniqueCodeResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GenerateUniqueCode")
            .Produces<GenerateUniqueCodeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate a unique code")
            .WithDescription("Generates a unique code with format CDCDN_RT_XXXXXXXX using timestamp and random values");
        }
    }
}
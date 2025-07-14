using BuildingBlocks.Strings;

namespace Inventory.API.Endpoints
{
    public record GenerateSupplierImportDocumentCodeResponse(string DocumentCode, string DocumentNumber);
    public class GenerateSupplierImportDocumentCodeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/supplier-import-documents/generate-code", async (ISender sender) =>
            {
                var result = await sender.Send(new GenerateSupplierImportDocumentCodeQuery());

                var response = result.Adapt<GenerateSupplierImportDocumentCodeResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GenerateSupplierImportDocumentCode")
            .Produces<GenerateSupplierImportDocumentCodeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate a document code for supplier import")
            .WithDescription("Generates a document code with format PN[YYYYMMDD]-[Sequence]");
        }
    }
}

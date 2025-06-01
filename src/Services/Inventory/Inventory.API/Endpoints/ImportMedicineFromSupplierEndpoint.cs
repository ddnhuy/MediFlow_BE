namespace Inventory.API.Endpoints
{
    public record ImportMedicineFromSupplierResponse(int SupplierImportDocumentId);
    public class ImportMedicineFromSupplierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/import-medicine-from-supplier", async (ImportMedicineFromSupplierCommand command, ISender mediator) =>
            {
                var result = await mediator.Send(command);
                var response = result.Adapt<ImportMedicineFromSupplierResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ImportMedicineFromSupplier")
            .Produces<ImportMedicineFromSupplierResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("Medicines")
            .WithSummary("Import medicines from a supplier");
        }
    }
}

using BuildingBlocks.Strings.Enums;
using Inventory.Application.Medicines.Queries.GetMedicineBatchReturnById;

namespace Inventory.API.Endpoints
{
    public record GetMedicineBatchReturnByIdResponse(
        int Id,
        string ReturnCode,
        string? Reason,
        string ReceiverName,
        string ReceiverPhone,
        string ReceiverEmail,
        MedicineBatchReturnStatus Status,
        DateTime? ApprovedAt,
        DateTime? RejectedAt,
        DateTime? CreatedAt,
        List<MedicineBatchReturnDetailItemResponse> Details
    );

    public record MedicineBatchReturnDetailItemResponse(
        int Id,
        int MedicineBatchId,
        string BatchNumber,
        DateOnly ExpirationDate,
        decimal Quantity,
        int SupplierId,
        string SupplierName,
        string ContactPerson,
        string PhoneNumber,
        string Email
    );

    public class GetMedicineBatchReturnByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-batch-returns/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicineBatchReturnByIdQuery(id));

                var response = new GetMedicineBatchReturnByIdResponse(
                    Id: result.MedicineBatchReturn.Id,
                    ReturnCode: result.MedicineBatchReturn.ReturnCode,
                    Reason: result.MedicineBatchReturn.Reason,
                    ReceiverName: result.MedicineBatchReturn.ReceiverName,
                    ReceiverPhone: result.MedicineBatchReturn.ReceiverPhone,
                    ReceiverEmail: result.MedicineBatchReturn.ReceiverEmail,
                    Status: result.MedicineBatchReturn.Status,
                    ApprovedAt: result.MedicineBatchReturn.ApprovedAt,
                    RejectedAt: result.MedicineBatchReturn.RejectedAt,
                    CreatedAt: result.MedicineBatchReturn.CreatedAt,
                    Details: result.MedicineBatchReturn.Details.Select(d => new MedicineBatchReturnDetailItemResponse(
                        Id: d.Id,
                        MedicineBatchId: d.MedicineBatchId,
                        BatchNumber: d.BatchNumber,
                        ExpirationDate: d.ExpirationDate,
                        Quantity: d.Quantity,
                        SupplierId: d.SupplierId,
                        SupplierName: d.SupplierName,
                        ContactPerson: d.ContactPerson,
                        PhoneNumber: d.PhoneNumber,
                        Email: d.Email
                    )).ToList()
                );

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicineBatchReturnById")
            .Produces<GetMedicineBatchReturnByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get medicine batch return request by ID")
            .WithDescription("Retrieves detailed information of a specific medicine batch return request including all return details");
        }
    }
}
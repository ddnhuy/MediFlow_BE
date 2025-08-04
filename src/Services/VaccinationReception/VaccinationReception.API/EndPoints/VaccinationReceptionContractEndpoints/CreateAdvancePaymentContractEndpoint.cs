using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record CreateAdvancePaymentContractRequest(
        decimal AdvanceAmount,
        PaymentMethod PaymentMethod,
        string? VATInvoiceNumber,
        string TaxCode,
        string OrganizationName
    );
    public record CreateAdvancePaymentContractResponse(
        int Id,
        string InvoiceNumber,
        string? VATInvoiceNumber,
        InvoiceType InvoiceType,
        decimal TotalAmount,
        PaymentMethod PaymentMethod,
        PaymentStatus? Status,
        string? TaxCode,
        string OrganizationName,
        string? ATMCode

    );

    public class CreateAdvancePaymentContractEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/contracts/{contractId}/advance-payment",
                         async (
                             int contractId,
                             [FromBody] CreateAdvancePaymentContractRequest request,
                             ISender sender) =>
                         {
                    var command = new CreateAdvancePaymentContractCommand(
                        ContractId: contractId,
                        AdvanceAmount: request.AdvanceAmount,
                        PaymentMethod: request.PaymentMethod,
                        VATInvoiceNumber: request.VATInvoiceNumber,
                        TaxCode: request.TaxCode,
                        OrganizationName: request.OrganizationName
                    );

                    var result = await sender.Send(command);

                     var response = new CreateAdvancePaymentContractResponse(
                         result.PaymentContract.Id,
                         result.PaymentContract.InvoiceNumber,
                         result.PaymentContract.VATInvoiceNumber,
                         result.PaymentContract.InvoiceType,
                         result.PaymentContract.TotalAmount,
                         result.PaymentContract.PaymentMethod,
                         result.PaymentContract.Status,
                         result.PaymentContract.TaxCode,
                         result.PaymentContract.OrganizationName,
                         result.PaymentContract.ATMCode
                     );
                     return Results.Created($"/contracts/{contractId}/advance-payment/{result.PaymentContract.Id}", response);
                })
            .RequireAuthorization()
            .WithName("CreateAdvancePaymentContract")
            .WithTags("Contract")
            .Produces<CreateAdvancePaymentContractResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create advance payment contract")
            .WithDescription("Creates an advance payment contract for a specific contract. Validates contract existence and status, generates invoice number, and creates payment record.");
        }
    }
}

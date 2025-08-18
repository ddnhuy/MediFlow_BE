using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record CreateFinalInvoicePaymentContractRequest(
        PaymentMethod PaymentMethod,
        string? VATInvoiceNumber,
        string TaxCode,
        string OrganizationName
    );
    public record CreateFinalInvoicePaymentContractResponse(
        int ContractId,
        PaymentContractDTO PaymentContract,
        List<ContractServiceDetailDTO> Details,
        string? CheckoutUrl,
        string? QrCode
    );
    public class CreateFinalInvoicePaymentContractEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/contracts/{contractId}/final-invoice-payment",
                async (int contractId, [FromBody] CreateFinalInvoicePaymentContractRequest request, ISender sender) =>
                {
                    var command = new CreateFinalInvoicePaymentContractCommand(
                        contractId,
                        request.PaymentMethod,
                        request.VATInvoiceNumber,
                        request.TaxCode,
                        request.OrganizationName
                    );

                    var result = await sender.Send(command);

                    var response = new CreateFinalInvoicePaymentContractResponse(
                        result.ContractId,
                        result.PaymentContract,
                        result.Details,
                        result.CheckoutUrl,
                        result.QrCode
                    );

                    return Results.Created($"/contracts/{contractId}/final-invoice-payment/{result.PaymentContract.Id}", response);
                })
            .WithName("CreateFinalInvoicePaymentContract")
            .RequireAuthorization()
            .WithName("CreateFinalInvoicePaymentContract")
            .WithTags("Contract")
            .Produces<CreateFinalInvoicePaymentContractResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create final invoice payment contract")
            .WithDescription("Creates a final invoice payment contract, updates actual quantities and amounts for contract service details, and updates contract actual amount.");
        }
    }
}

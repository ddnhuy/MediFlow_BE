using VaccinationReception.Application.DTOs.HospitalFeeDTOs;
using VaccinationReception.Application.HospitalFees.Queries;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record GetAllPaymentsWithPatientsResponse(PaginatedResult<PaymentWithPatientDTO> Payments);

    public class GetAllPaymentsWithPatientsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/payments", async ([AsParameters] PaginationRequest request, string? searchTerm, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var result = await sender.Send(new GetAllPaymentsWithPatientsQuery(request, searchTerm));
                var response = new GetAllPaymentsWithPatientsResponse(result);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllPaymentsWithPatients")
            .WithTags("Payment")
            .Produces<GetAllPaymentsWithPatientsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get payments with patient info")
            .WithDescription("Returns payments joined with patient info. Applies search (Name/Code/IdentityCard/InvoiceNumber) at the end, then paginates.");
        }
    }
}

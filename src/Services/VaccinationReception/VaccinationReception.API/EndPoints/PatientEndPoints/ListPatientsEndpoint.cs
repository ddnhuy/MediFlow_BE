namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record ListPatientsResponse(PaginatedResult<PatientSummaryDTO> Patients);
    public class ListPatientsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients", async (
                [AsParameters] PaginationRequest request,
                string? name,
                string? code,
                string? identityCard,
                string? phoneNumber,
                ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var result = await sender.Send(new ListPatientsQuery(request, name, code, identityCard, phoneNumber));

                if (result == null || result.Patients.Data == null || !result.Patients.Data.Any())
                {
                    return Results.Ok(new ListPatientsResponse(new PaginatedResult<PatientSummaryDTO>(request.PageIndex, request.PageSize, 0, new List<PatientSummaryDTO>())));
                }

                var response = result.Adapt<ListPatientsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ListPatients")
            .Produces<ListPatientsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all patients")
            .WithDescription("Get all patients with pagination support");
        }
    }
}
namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record ListPatientsResponse(PaginatedResult<PatientSummaryDTO> Patients);
    public class ListPatientsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                if (request.PageIndex <= 0 || request.PageSize <= 0)
                {
                    return Results.Problem(
                        title: "BadRequest",
                        detail: "Pagination parameters must be greater than zero.",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                var result = await sender.Send(new ListPatientsQuery(request));

                if (result == null || result.Patients.Data == null || !result.Patients.Data.Any())
                {
                    return Results.Problem(
                        title: "NotFound",
                        detail: "No patients found.",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }

                var response = result.Adapt<ListPatientsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ListPatients")
            .Produces<ListPatientsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all patients")
            .WithDescription("Get all patients with pagination support");
        }
    }
}
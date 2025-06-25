namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record GetListPatientsUnpaidServiceResponse(IEnumerable<PatientDTO> Patients);

    public class GetListPatientsUnpaidServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/unpaid-services", async (
                [AsParameters] GetListPatientsUnpaidServiceQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(new GetListPatientsUnpaidServiceResponse(result.Patients));
            })
            .RequireAuthorization()
            .WithName("GetListPatientsUnpaidService")
            .Produces<GetListPatientsUnpaidServiceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Patients with Unpaid Services")
            .WithDescription("Returns a list of patients who have unpaid vaccination or service requests.");
        }
    }
}

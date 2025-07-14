using VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetVaccinationHistoryByPatientIdResponse(
       string PatientCode,
       string PatientVaccinationCode,
       string PatientName,
       string Gender,
       string PhoneNumber,
       string AddressDetail,
       string Ward,
       string District,
       string Province,
       List<VaccinationHistoryItem> VaccinationHistoryItems
   );
    public class GetVaccinationHistoryByPatientIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/patient/{patientId}/history", async (int patientId, ISender sender) =>
            {
                var query = new GetVaccinationHistoryByPatientIdQuery(patientId);
                var result = await sender.Send(query);
                var response = result.Adapt<GetVaccinationHistoryByPatientIdResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetVaccinationHistoryByPatientId")
            .Produces<GetVaccinationHistoryByPatientIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Vaccination History By Patient Id")
            .WithDescription("Retrieves the complete vaccination history for a specific patient");
        }
    }
}

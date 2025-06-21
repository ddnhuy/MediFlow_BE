using VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetMedicineListForVaccinationByReceptionIdResponse(
        int MedicineId,
        string MedicineName
    );

    public class GetMedicineListForVaccinationByReceptionIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/reception/{receptionId}/medicines", async (int receptionId, ISender sender) =>
            {
                var query = new GetMedicineListForVaccinationByReceptionIdQuery(receptionId);
                var result = await sender.Send(query);
                var response = result.Adapt<List<GetMedicineListForVaccinationByReceptionIdResponse>>();
                return Results.Ok(response);
            }).RequireAuthorization()
              .WithName("GetMedicineListForVaccinationByReceptionId")
              .Produces<List<GetMedicineListForVaccinationByReceptionIdResponse>>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithSummary("Get Medicine List For Vaccination By Reception Id")
              .WithDescription("Get Medicine List For Vaccination By Reception Id");
        }
    }
}

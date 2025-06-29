using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record class GetMedicineListForPreExaminationResponse(List<PreExaminationMedicineItem> PreExaminationMedicineItems);
    public class GetMedicineListForPreExaminationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/pre-examination/reception/{receptionId}/medicines", async (int receptionId, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetMedicineListForPreExaminationQuery(receptionId);
                var result = await sender.Send(query, cancellationToken);
                var response = new GetMedicineListForPreExaminationResponse(result.PreExaminationMedicineItems);    
                return Results.Ok(result);
            }) .RequireAuthorization()
              .WithName("GetMedicineListForPreExamination")
              .Produces<GetMedicineListForPreExaminationResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get list of medicines for pre-examination testing")
              .WithDescription("Retrieves a list of medicines used in pre-examination testing for a specific reception.");
        }
    }
}

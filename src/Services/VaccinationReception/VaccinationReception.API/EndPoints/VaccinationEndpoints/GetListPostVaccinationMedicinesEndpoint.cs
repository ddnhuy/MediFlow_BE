
using VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationMedicines;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetListPostVaccinationMedicinesResponse(List<GetListPostVaccinationMedicinesResult> Medicines);
    public class GetListPostVaccinationMedicinesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/post-vaccination/reception/{receptionId}/medicines", async (int receptionId, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetListPostVaccinationMedicinesQuery(receptionId);
                var result = await sender.Send(query, cancellationToken);
                var response = new GetListPostVaccinationMedicinesResponse(result);
                return Results.Ok(result);
            }).RequireAuthorization()
              .WithName("GetListPostVaccinationMedicines")
              .Produces<GetListPostVaccinationMedicinesResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get list of post-vaccination medicines")
              .WithDescription("Retrieves a list of medicines used in post-vaccination for a specific reception.");
        }
    }
}

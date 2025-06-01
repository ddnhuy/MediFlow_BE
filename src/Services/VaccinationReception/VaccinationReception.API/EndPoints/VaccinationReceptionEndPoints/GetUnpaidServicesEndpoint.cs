using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class GetUnpaidServicesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/{receptionId}/unpaid-services", async (
                int receptionId,
                ISender sender) =>
            {
                try
                {
                    if (receptionId <= 0)
                    {
                        return Results.BadRequest("Reception ID không hợp lệ");
                    }

                    var query = new GetUnpaidServicesQuery(receptionId);
                    var result = await sender.Send(query);

                    if (result == null ||
                        (!result.Services.Any() && !result.Vaccinations.Any()))
                    {
                        return Results.NotFound("Không tìm thấy unpaid services");
                    }

                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }
            })
            .RequireAuthorization()
            .WithName("GetUnpaidServices")
            .Produces<UnpaidServicesResponseDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get unpaid services and vaccinations")
            .WithDescription("Retrieves a list of unpaid services and vaccinations for a specific reception");
        }
    }
}
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record GetReceptionVaccinationsByReceptionIdResponse(IEnumerable<ReceptionVaccinationDTO> ReceptionVaccinations);

    public class GetReceptionVaccinationsByReceptionIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/{receptionId}/vaccinations", async (int receptionId, ISender sender) =>
            {
                if (receptionId <= 0)
                {
                    return Results.BadRequest("Reception ID không hợp lệ");
                }

                var query = new GetReceptionVaccinationsByReceptionIdQuery(receptionId);
                var result = await sender.Send(query);

                if (result == null || !result.ReceptionVaccinations.Any())
                {
                    return Results.NotFound($"Không tìm thấy lịch tiêm cho lần tiếp nhận {receptionId}");
                }

                var response = result.Adapt<GetReceptionVaccinationsByReceptionIdResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetReceptionVaccinationsByReceptionId")
            .Produces<GetReceptionVaccinationsByReceptionIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get reception vaccinations by reception ID")
            .WithDescription("Retrieves all vaccination records associated with a specific reception");
        }
    }
}
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record DeleteReceptionVaccinationsResponse(bool IsSuccess, int DeletedCount);

    public class DeleteReceptionVaccinationsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/reception-vaccinations", async ([FromBody] List<int> receptionVaccinationIds, ISender sender) =>
            {
                if (receptionVaccinationIds == null || !receptionVaccinationIds.Any())
                {
                    return Results.BadRequest("Danh sách ID không được để trống");
                }

                var command = new DeleteReceptionVaccinationsCommand(receptionVaccinationIds);
                var result = await sender.Send(command);

                if (!result.IsSuccess)
                {
                    return Results.NotFound($"Không tìm thấy ReceptionVaccination nào để xóa");
                }

                var response = result.Adapt<DeleteReceptionVaccinationsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteReceptionVaccinations")
            .Produces<DeleteReceptionVaccinationsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete reception vaccinations")
            .WithDescription("Soft deletes one or more reception vaccination records");
        }
    }
}
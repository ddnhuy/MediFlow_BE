using BuildingBlocks.Strings;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record DeleteReceptionVaccinationsResponse(bool IsSuccess, int DeletedCount);

    public class DeleteReceptionVaccinationsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/reception-vaccinations/{receptionId}", async (
                 int receptionId,
                [FromBody] List<int> receptionVaccinationIds, 
                ISender sender) =>
            {
                if (receptionVaccinationIds == null || !receptionVaccinationIds.Any())
                {
                    throw new ArgumentException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID_LIST.ToString());
                }

                var command = new DeleteReceptionVaccinationsCommand(receptionId, receptionVaccinationIds);
                var result = await sender.Send(command);

                if (!result.IsSuccess)
                {
                    throw new NotFoundException(ExceptionKey.FAILED_DELETE_VACCINATION_RECEPTION);
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
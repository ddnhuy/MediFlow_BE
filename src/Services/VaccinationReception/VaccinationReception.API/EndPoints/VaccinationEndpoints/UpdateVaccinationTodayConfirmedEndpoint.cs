using VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationTodayConfirmed;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record ConfirmVaccinationTodayResponse(bool IsSuccess);

    public class UpdateVaccinationTodayConfirmedEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("vaccination/receptions/{receptionId}/confirm-vaccination-today",
                async (int receptionId, ISender sender) =>
                {
                    var command = new ConfirmVaccinationTodayCommand(receptionId);

                    var result = await sender.Send(command);

                    var response = new ConfirmVaccinationTodayResponse(result.IsSuccess);
                    return Results.Ok(response);
                })
            .RequireAuthorization()
            .WithName("ConfirmVaccinationToday")
            .Produces<ConfirmVaccinationTodayResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Confirm all vaccinations for a reception are done and observed")
            .WithDescription("Set IsVaccinationTodayConfirmed = true if all vaccinations in the reception are completed and observed.");
        }
    }
}

using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record UpdateReceptionVaccinationResponse(bool IsSuccess);

    public class UpdateReceptionVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/receptionvaccinations/{id}", async (int id, [FromBody] UpdateReceptionVaccinationCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID trong đường dẫn không khớp với ID trong nội dung yêu cầu");
                }

                var result = await sender.Send(command);

                if (!result.IsSuccess)
                {
                    return Results.NotFound($"Không tìm thấy ReceptionVaccination với Id: {id}");
                }

                var response = result.Adapt<UpdateReceptionVaccinationResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateReceptionVaccination")
            .Produces<UpdateReceptionVaccinationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update a reception vaccination")
            .WithDescription("Updates an existing reception vaccination record");
        }
    }
}
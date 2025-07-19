using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record AddServiceToRequestFormResponse(int RequestFormId, string RequestNumber);
    public record AddServiceToRequestFormResultResponse(
        int ReceptionId,
        List<ServiceIdAndRequestNumberDTO> ProcessedServiceReferences
    );

    public class AddServiceToRequestFormEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/request-forms/add-service", async (AddServiceToRequestFormCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                var response = result.Adapt<AddServiceToRequestFormResultResponse>();
                return Results.Created($"/request-forms/{response.ReceptionId}", response);
            })
            .RequireAuthorization()
            .WithName("AddServiceToRequestForm")
            .Produces<AddServiceToRequestFormResultResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add services to request form")
            .WithDescription("Adds services to an existing request form or creates a new one");
        }
    }
}
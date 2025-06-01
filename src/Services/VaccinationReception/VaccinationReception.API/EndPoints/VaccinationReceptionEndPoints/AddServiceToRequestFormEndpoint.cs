using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record AddServiceToRequestFormResponse(int RequestFormId, string RequestNumber);

    public class AddServiceToRequestFormEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/request-forms/add-service", async (AddServiceToRequestFormCommand command, ISender sender) =>
            {
                try
                {
                    var result = await sender.Send(command);
                    var response = result.Adapt<AddServiceToRequestFormResponse>();
                    return Results.Created($"/request-forms/{response.RequestFormId}", response);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }
            })
            .RequireAuthorization()
            .WithName("AddServiceToRequestForm")
            .Produces<AddServiceToRequestFormResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add services to request form")
            .WithDescription("Adds services to an existing request form or creates a new one");
        }
    }
}
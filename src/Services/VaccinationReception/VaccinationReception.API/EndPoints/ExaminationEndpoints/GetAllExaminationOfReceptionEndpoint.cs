
using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetAllExaminationOfReceptionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/reception/{receptionId}/examination", async (int receptionId, IMediator mediator) =>
            {
                var query = new GetAllExaminationOfReceptionQuery(receptionId);
                var response = await mediator.Send(query);
                return Results.Ok(response);
            }).RequireAuthorization()
            .WithName("GetAllExaminationOfReception")
            .Produces<GetAllExaminationOfReceptionQueryResponse>(StatusCodes.Status200OK)
            .WithSummary("Get all examinations of a specific reception by ID");
        }
    }
}

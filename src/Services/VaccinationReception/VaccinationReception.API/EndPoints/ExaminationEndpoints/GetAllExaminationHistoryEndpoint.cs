using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetAllExaminationHistoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/examination/history", async (ISender mediator, [AsParameters] PaginationRequest paginationRequest, string? searchTerm) =>
            {
                var query = new GetAllExaminationHistoryQuery(paginationRequest, searchTerm);
                var response = await mediator.Send(query);
                return Results.Ok(response);
            }).RequireAuthorization()
            .Produces<GetAllExaminationHistoryResponse>()
            .WithName("GetAllExaminationHistory");
        }
    }
}

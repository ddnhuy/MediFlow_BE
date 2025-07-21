
using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetServiceTestParametersOfExaminationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/examinations/{examinationId}/service-test-parameters", async (int examinationId, ISender sender) =>
            {
                var query = new GetServiceTestParametersOfExaminationQuery(examinationId);
                var response = await sender.Send(query);
                return Results.Ok(response);
            }).RequireAuthorization()
            .Produces<GetServiceTestParametersOfExaminationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetServiceTestParametersOfExamination");
        }
    }
}

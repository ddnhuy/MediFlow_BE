
using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetPatientExaminationDetailEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/examination/{examinationId:int}/patient-detail", async (int examinationId, ISender mediator) =>
            {
                var query = new GetPatientExaminationDetailQuery(examinationId);
                var response = await mediator.Send(query);
                if (response == null)
                {
                    return Results.NotFound(new { Message = "Examination not found." });
                }
                return Results.Ok(response);
            }).RequireAuthorization()
            .WithName("GetPatientExaminationDetail")
            .WithSummary("Get details of a specific patient examination by ID")
            .Produces<GetPatientExaminationDetailQueryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Examinations");           
        }
    }
}


using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetExaminationDetailOfPatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("examination/{examinationId:int}/examination-result", async (int examinationId, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetExaminationDetailOfPatientQuery(examinationId);
                var response = await sender.Send(query, cancellationToken);
                return Results.Ok(response);
            }).RequireAuthorization()
            .WithName("GetExaminationDetailOfPatient")
            .WithSummary("Get examination detail of a patient by examination ID")
            .Produces<GetExaminationDetailOfPatientQueryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}


using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetAllExaminationHistoryOfPatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/examination/history/patient/{patientId:int}", async (ISender sender, int patientId) =>
            {
                var query = new GetAllExaminationHistoryOfPatientQuery(patientId);
                var response = await sender.Send(query);
                return Results.Ok(response);
            }).RequireAuthorization()
            .Produces<GetAllExaminationHistoryOfPatientResponse>()
            .WithName("GetAllExaminationHistoryOfPatient")
            .WithSummary("Get All Examination History of Patient");
        }
    }
}

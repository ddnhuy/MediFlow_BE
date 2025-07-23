
using VaccinationReception.Application.Examinations.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetAllExaminationTechnicianEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/examination/users", async (ISender sender, string roleName, CancellationToken cancellationToken) =>
            {
                var query = new GetAllExaminationTechnicianQuery(roleName);
                var response = await sender.Send(query, cancellationToken);
                return Results.Ok(response);
            }).RequireAuthorization()
            .Produces<GetAllExaminationTechnicianRespone>(StatusCodes.Status200OK);           
        }
    }
}

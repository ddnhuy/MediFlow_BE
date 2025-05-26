namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record GetPatientByIdResponse(PatientDetailDTO Patient);
    public class GetPatientByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{id}", async (int id, ISender sender) =>
            {
                if(id <= 0)
                {
                    return Results.BadRequest("Id không hợp lệ");
                }

                var query = new GetPatientQuery(id);
                var result = await sender.Send(query);

                if (result == null)
                {
                    return Results.NotFound($"Không tìm thấy bệnh nhân với ID {id}");
                }

                return Results.Ok(new GetPatientByIdResponse(result.Patient));
            })
            .RequireAuthorization()
            .WithName("GetPatientById")
            .Produces<GetPatientByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get patient by Id")
            .WithDescription("Get patient details by Id");
        }
    }
}
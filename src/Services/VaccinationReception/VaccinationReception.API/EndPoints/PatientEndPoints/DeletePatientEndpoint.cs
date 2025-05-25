using Carter;
using Mapster;
using MediatR;
using VaccinationReception.Application.Patients.Commands.DeletePatient;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record DeletePatientResponse(bool IsSuccess);
    public class DeletePatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/patients/{id}", async (int id, ISender sender) =>
            {
                var command = new DeletePatientCommand(id);
                var result = await sender.Send(command);

                var response = result.Adapt<DeletePatientResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeletePatient")
            .Produces<DeletePatientResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a patient")
            .WithDescription("Soft deletes a patient by marking it as cancelled");
        }
    }
}
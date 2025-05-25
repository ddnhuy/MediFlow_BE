using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record UpdatePatientResponse(bool IsSuccess);
    public class UpdatePatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/patients/{id}", async (int id, [FromBody] UpdatePatientCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID trong đường dẫn không khớp với ID trong nội dung yêu cầu");
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdatePatientResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdatePatient")
            .Produces<UpdatePatientResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing patient")
            .WithDescription("Updates an existing patient record");
        }
    }
}
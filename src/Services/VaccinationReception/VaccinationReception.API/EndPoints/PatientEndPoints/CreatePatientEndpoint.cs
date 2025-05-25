using BuildingBlocks.Exceptions;
using Carter;
using Mapster;
using MediatR;
using VaccinationReception.Application.Patients.Commands.CreatePatient;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record CreatePatientResponse(int Id);
    public class CreatePatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/patients", async (CreatePatientCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Tạo bệnh nhân thất bại");
                }

                var response = result.Adapt<CreatePatientResponse>();
                return Results.Created($"/patients/{response.Id}", response);
            })
            .RequireAuthorization()
            .WithName("CreatePatient")
            .Produces<CreatePatientResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new patient")
            .WithDescription("Creates a new patient record");
        }
    }
}
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record CreateExaminationServiceResponse(int ServiceId);

    public class CreateExaminationServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/services/examination", async ([FromBody] CreateExaminationServiceCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                var response = result.Adapt<CreateExaminationServiceResponse>();
                return Results.Created($"/services/{response.ServiceId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateExaminationService")
            .Produces<CreateExaminationServiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new examination service")
            .WithDescription("Creates a new examination service record with parameters and enum type.");
        }
    }
}
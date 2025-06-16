using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record CreateServiceResponse(int ServiceId);

    public class CreateServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/services", async ([FromBody] CreateServiceCommand command, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_CREATE_SERVICE);
                }

                var response = result.Adapt<CreateServiceResponse>();
                return Results.Created($"/services/{response.ServiceId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateService")
            .Produces<CreateServiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new service")
            .WithDescription("Creates a new service record");
        }
    }
}
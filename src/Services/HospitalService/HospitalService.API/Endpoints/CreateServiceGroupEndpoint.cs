using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record CreateServiceGroupResponse(int ServiceGroupId);
    public class CreateServiceGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/service-groups", async ([FromBody] CreateServiceGroupCommand command, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_CREATE_SERVICE_GROUP);
                }

                var response = result.Adapt<CreateServiceGroupResponse>();
                return Results.Created($"/service-groups/{response.ServiceGroupId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateServiceGroup")
            .Produces<CreateServiceGroupResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new service group")
            .WithDescription("Creates a new service group record");
        }
    }
}
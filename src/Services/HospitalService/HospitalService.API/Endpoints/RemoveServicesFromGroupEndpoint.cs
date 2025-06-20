using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record RemoveServicesFromGroupResponse(int ServiceGroupId, int RemovedServicesCount);
    public class RemoveServicesFromGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/service-groups/{id}/services", async (int id, [FromBody] RemoveServicesFromGroupCommand command, ISender sender) =>
            {
                if (id <= 0) throw new BadRequestException(ExceptionKey.INVALID_SERVICE_GROUP_ID);
                if (command.ServiceIds?.Any() != true) throw new BadRequestException(ExceptionKey.EMPTY_SERVICE_IDS);

                command = command with { ServiceGroupId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_REMOVE_SERVICES_FROM_GROUP);
                }

                var response = result.Adapt<RemoveServicesFromGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("RemoveServicesFromGroup")
            .Produces<RemoveServicesFromGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove services from a group")
            .WithDescription("Removes one or more services from an existing service group");
        }
    }
}

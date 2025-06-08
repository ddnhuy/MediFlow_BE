using BuildingBlocks.Exceptions;
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
            app.MapDelete("/servicegroups/{id}/services", async (int id, [FromBody] RemoveServicesFromGroupCommand command, ISender sender) =>
            {
                if (id <= 0) return Results.BadRequest("ServiceGroupId must be greater than zero.");
                if (command.ServiceIds?.Any() != true) return Results.BadRequest("ServiceIds cannot be null or empty.");

                command = command with { ServiceGroupId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Xóa dịch vụ khỏi nhóm thất bại");
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

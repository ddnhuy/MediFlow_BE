using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record UpdateServiceGroupResponse(int ServiceGroupId);
    public class UpdateServiceGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/service-groups/{id}", async (int id, [FromBody] UpdateServiceGroupCommand command, ISender sender) =>
            {
                if (id <= 0) throw new BadRequestException(ExceptionKey.INVALID_SERVICE_GROUP_ID);
                if (string.IsNullOrWhiteSpace(command.GroupName)) throw new BadRequestException(ExceptionKey.EMPTY_GROUP_NAME);

                command = command with { Id = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_UPDATE_SERVICE_GROUP);
                }

                var response = result.Adapt<UpdateServiceGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateServiceGroup")
            .Produces<UpdateServiceGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update a service group")
            .WithDescription("Updates the name of an existing service group");
        }
    }
}

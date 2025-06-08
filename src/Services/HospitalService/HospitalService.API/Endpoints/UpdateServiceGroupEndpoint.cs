using BuildingBlocks.Exceptions;
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
            app.MapPut("/servicegroups/{id}", async (int id, [FromBody] UpdateServiceGroupCommand command, ISender sender) =>
            {
                if (id <= 0) return Results.BadRequest("Id must be greater than zero.");
                if (string.IsNullOrWhiteSpace(command.GroupName)) return Results.BadRequest("GroupName cannot be empty.");

                command = command with { Id = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Cập nhật nhóm dịch vụ thất bại");
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

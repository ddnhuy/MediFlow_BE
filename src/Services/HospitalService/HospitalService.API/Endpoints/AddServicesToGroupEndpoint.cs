using BuildingBlocks.Exceptions;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record AddServicesToGroupResponse(int ServiceGroupId, int AddedServicesCount);
    public class AddServicesToGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/servicegroups/{id}/services", async (int id, [FromBody] AddServicesToGroupCommand command, ISender sender) =>
            {
                if (command.ServiceIds == null || !command.ServiceIds.Any())
                {
                    return Results.BadRequest("Danh sách dịch vụ không được để trống.");
                }

                command = command with { ServiceGroupId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Thêm dịch vụ vào nhóm thất bại");
                }

                var response = result.Adapt<AddServicesToGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("AddServicesToGroup")
            .Produces<AddServicesToGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add services to a group")
            .WithDescription("Adds one or more services to an existing service group");
        }
    }
}
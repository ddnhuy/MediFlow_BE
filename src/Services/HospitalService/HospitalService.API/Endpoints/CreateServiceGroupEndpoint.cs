using BuildingBlocks.Exceptions;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record CreateServiceGroupResponse(int ServiceGroupId);
    public class CreateServiceGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/servicegroups", async (CreateServiceGroupCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Tạo nhóm dịch vụ thất bại");
                }

                var response = result.Adapt<CreateServiceGroupResponse>();
                return Results.Created($"/servicegroups/{response.ServiceGroupId}", response);
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
using BuildingBlocks.Exceptions;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record DeleteServiceGroupResponse(int ServiceGroupId);
    public class DeleteServiceGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/servicegroups/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest("ID không hợp lệ. ID phải lớn hơn 0.");
                }

                var command = new DeleteServiceGroupCommand(Id: id);
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Xóa nhóm dịch vụ thất bại");
                }

                var response = result.Adapt<DeleteServiceGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteServiceGroup")
            .Produces<DeleteServiceGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a service group")
            .WithDescription("Soft deletes an existing service group");
        }
    }
}
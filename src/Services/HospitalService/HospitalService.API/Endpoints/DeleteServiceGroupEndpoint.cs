using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
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
            app.MapDelete("/service-groups/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_SERVICE_GROUP_ID);
                }

                var command = new DeleteServiceGroupCommand(Id: id);
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_DELETE_SERVICE_GROUP);
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
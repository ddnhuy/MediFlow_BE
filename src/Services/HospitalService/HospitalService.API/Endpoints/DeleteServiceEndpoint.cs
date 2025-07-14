using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record DeleteServiceResponse(int ServiceId);
    public class DeleteServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/services/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_SERVICE_ID);
                }

                var command = new DeleteServiceCommand(ServiceId: id);
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_DELETE_SERVICE);
                }

                var response = result.Adapt<DeleteServiceResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteService")
            .Produces<DeleteServiceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a service")
            .WithDescription("Soft deletes an existing service and its related records");
        }
    }
}
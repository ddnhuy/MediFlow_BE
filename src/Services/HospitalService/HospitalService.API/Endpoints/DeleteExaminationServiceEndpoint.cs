using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record DeleteExaminationServiceResponse(int ServiceId);

    public class DeleteExaminationServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/services/examination/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_SERVICE_ID);
                }

                var command = new DeleteExaminationServiceCommand(ServiceId: id);

                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_DELETE_SERVICE);
                }

                var response = result.Adapt<DeleteExaminationServiceResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteExaminationService")
            .WithTags("Service")
            .Produces<DeleteExaminationServiceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete an examination service")
            .WithDescription("Soft deletes an existing examination service and its related records including test parameters");
        }
    }
}
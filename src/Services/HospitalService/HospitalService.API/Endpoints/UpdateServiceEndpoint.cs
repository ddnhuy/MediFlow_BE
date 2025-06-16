using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record UpdateServiceResponse(int ServiceId);

    public class UpdateServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/services/{id}", async (int id, [FromBody] UpdateServiceCommand command, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_SERVICE_ID);
                }

                command = command with { ServiceId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_UPDATE_SERVICE);
                }

                var response = result.Adapt<UpdateServiceResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateService")
            .Produces<UpdateServiceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update a service")
            .WithDescription("Updates an existing service with the specified ID");
        }
    }
}

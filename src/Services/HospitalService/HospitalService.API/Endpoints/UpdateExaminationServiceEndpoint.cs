using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record UpdateExaminationServiceResponse(int ServiceId);

    public class UpdateExaminationServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/services/examination/{serviceId}", async (int serviceId, [FromBody] UpdateExaminationServiceCommand command, ISender sender) =>
            {
                if (serviceId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_SERVICE_ID);
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdateExaminationServiceResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateExaminationService")
            .Produces<UpdateExaminationServiceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an examination service")
            .WithDescription("Updates an existing examination service with the specified ID");
        }
    }
}
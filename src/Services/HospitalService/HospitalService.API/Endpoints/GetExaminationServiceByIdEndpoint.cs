using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public class GetExaminationServiceByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services/examination/{id}", async (
                int id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_SERVICE_ID);
                }

                var query = new GetExaminationServiceWithDetailsByIdQuery(id);
                var result = await sender.Send(query, cancellationToken);

                if (result == null)
                {
                    throw new NotFoundException(ExceptionKey.SERVICE_NOT_FOUND);
                }

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetExaminationServiceById")
            .WithTags("Service")
            .Produces<ExaminationServiceDetailDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get examination service by ID")
            .WithDescription("Get a specific examination service by its ID");
        }
    }
}
using BuildingBlocks.Exceptions;
using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public class GetServicesByIdsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/services/by-ids", async (
                [FromBody] List<int> serviceIds,
                ISender sender,
                ILogger<GetServicesByIdsEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                if (serviceIds == null || !serviceIds.Any())
                {
                    //throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_SERVICE_IDS);
                }

                if (serviceIds.Any(id => id <= 0))
                {
                   // throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_SERVICE_ID);
                }

                var query = new GetServicesByIdsQuery(serviceIds);
                var result = await sender.Send(query, cancellationToken);

                var response = result.Adapt<List<ServiceDTO>>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetServicesByIds")
            .WithTags("Service")
            .Produces<List<ServiceDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get services by IDs")
            .WithDescription("Get list of services by their IDs");
        }
    }
}

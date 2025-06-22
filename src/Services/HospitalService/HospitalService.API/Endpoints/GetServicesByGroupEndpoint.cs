using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public class GetServicesByGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services/group", async (
                [FromQuery] int groupId,
                [FromQuery] string groupType,
                ISender sender,
                ILogger<GetServicesByGroupEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                if (groupId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_GROUP_ID);
                }

                if (!string.Equals(groupType?.Trim(), GroupServiceType.SERVICE_GROUP, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(groupType?.Trim(), GroupServiceType.DISEASE_GROUP, StringComparison.OrdinalIgnoreCase))
                {
                     throw new BadRequestException(ExceptionKey.INVALID_GROUP_TYPE);
                }

                var query = new GetServicesByGroupQuery(groupId, groupType);
                var result = await sender.Send(query, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetServicesByGroup")
            .WithTags("ServiceGroup")
            .Produces<List<GetServicesByGroupResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get services by group")
            .WithDescription("Get list of services by service group or disease group")
            .RequireAuthorization();
        }
    }
}

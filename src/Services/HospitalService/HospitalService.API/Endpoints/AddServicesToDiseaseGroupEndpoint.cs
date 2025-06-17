using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record AddServicesToDiseaseGroupResponse(int DiseaseGroupId, int AddedServicesCount);
    public class AddServicesToDiseaseGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/disease-groups/{id}/services", async (int id, [FromBody] AddServicesToDiseaseGroupCommand command, ISender sender) =>
            {
                if (command.ServiceIds == null || !command.ServiceIds.Any())
                {
                    throw new BadRequestException(HospitalServiceExceptionStrings.EMPTY_SERVICE_IDS);
                }

                command = command with { DiseaseGroupId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_ADD_SERVICES_TO_DISEASE_GROUP);
                }

                var response = result.Adapt<AddServicesToDiseaseGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("AddServicesToDiseaseGroup")
            .Produces<AddServicesToDiseaseGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add services to a disease group")
            .WithDescription("Adds one or more services to an existing disease group");
        }
    }
}

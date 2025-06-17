using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record RemoveServicesFromDiseaseGroupResponse(int DiseaseGroupId, int RemovedServicesCount);
    public class RemoveServicesFromDiseaseGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/disease-groups/{id}/services", async (int id, [FromBody] RemoveServicesFromDiseaseGroupCommand command, ISender sender) =>
            {
                if (id <= 0) throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_DISEASE_GROUP_ID);
                if (command.ServiceIds?.Any() != true) throw new BadRequestException(HospitalServiceExceptionStrings.EMPTY_SERVICE_IDS);

                command = command with { DiseaseGroupId = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_REMOVE_SERVICES_FROM_DISEASE_GROUP);
                }

                var response = result.Adapt<RemoveServicesFromDiseaseGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("RemoveServicesFromDiseaseGroup")
            .Produces<RemoveServicesFromDiseaseGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove services from a disease group")
            .WithDescription("Removes one or more services from an existing disease group");
        }
    }
}

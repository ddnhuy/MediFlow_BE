using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record UpdateDiseaseGroupResponse(int DiseaseGroupId);
    public class UpdateDiseaseGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/disease-groups/{id}", async (int id, [FromBody] UpdateDiseaseGroupCommand command, ISender sender) =>
            {
                if (id <= 0) throw new BadRequestException(ExceptionKey.INVALID_DISEASE_GROUP_ID);
                if (string.IsNullOrWhiteSpace(command.GroupName)) throw new BadRequestException(ExceptionKey.EMPTY_DISEASE_GROUP_NAME);

                command = command with { Id = id };
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_UPDATE_DISEASE_GROUP);
                }

                var response = result.Adapt<UpdateDiseaseGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateDiseaseGroup")
            .Produces<UpdateDiseaseGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update a disease group")
            .WithDescription("Updates the name and description of an existing disease group");
        }
    }
}

using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalService.API.Endpoints
{
    public record CreateDiseaseGroupResponse(int DiseaseGroupId);
    public class CreateDiseaseGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/disease-groups", async ([FromBody] CreateDiseaseGroupCommand command, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(ExceptionKey.FAILED_CREATE_DISEASE_GROUP);
                }

                var response = result.Adapt<CreateDiseaseGroupResponse>();
                return Results.Created($"/disease-groups/{response.DiseaseGroupId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateDiseaseGroup")
            .Produces<CreateDiseaseGroupResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new disease group")
            .WithDescription("Creates a new disease group record");
        }
    }
}

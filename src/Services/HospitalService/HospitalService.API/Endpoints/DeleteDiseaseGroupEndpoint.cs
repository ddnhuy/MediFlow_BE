using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings.ExceptionStrings;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record DeleteDiseaseGroupResponse(int DiseaseGroupId);
    public class DeleteDiseaseGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/disease-groups/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(HospitalServiceExceptionStrings.INVALID_DISEASE_GROUP_ID);
                }

                var command = new DeleteDiseaseGroupCommand(Id: id);
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException(HospitalServiceExceptionStrings.FAILED_DELETE_DISEASE_GROUP);
                }

                var response = result.Adapt<DeleteDiseaseGroupResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteDiseaseGroup")
            .Produces<DeleteDiseaseGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a disease group")
            .WithDescription("Soft deletes an existing disease group");
        }
    }
}

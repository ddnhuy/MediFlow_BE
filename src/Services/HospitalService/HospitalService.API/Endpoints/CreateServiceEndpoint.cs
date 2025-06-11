using BuildingBlocks.Exceptions;
using Carter;
using HospitalService.Application.Services.HospitalServices.Commands;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record CreateServiceResponse(int ServiceId);

    public class CreateServiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/services", async (CreateServiceCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Tạo dịch vụ thất bại");
                }

                var response = result.Adapt<CreateServiceResponse>();
                return Results.Created($"/services/{response.ServiceId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateService")
            .Produces<CreateServiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new service")
            .WithDescription("Creates a new service record");
        }
    }
}
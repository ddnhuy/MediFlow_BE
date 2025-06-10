using Microsoft.AspNetCore.Mvc;

namespace FileStorage.API.Endpoints
{
    public record DeleteImageResponse(bool IsSuccess);

    public class DeleteImageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/images/delete", async ([FromQuery] string imageUrl, ISender sender) =>
            {
                var result = await sender.Send(new DeleteImageCommand(imageUrl));

                return Results.Ok(result);
            })
            .WithName("DeleteImage")
            .Produces<DeleteImageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Image")
            .WithDescription("Delete Image");
        }
    }
}

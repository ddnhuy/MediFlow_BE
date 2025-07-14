namespace FileStorage.API.Endpoints
{
    public record DeleteFileResponse(bool IsSuccess);

    public class DeleteFileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/files/{fileId}", async (Guid fileId, ISender sender) =>
            {
                var result = await sender.Send(new DeleteFileCommand(fileId));

                return Results.Ok(result.Adapt<DeleteFileResponse>());
            })
            .WithName("DeleteFile")
            .Produces<DeleteFileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete File")
            .WithDescription("Delete File");
        }
    }
}

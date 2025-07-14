namespace FileStorage.API.Endpoints
{
    public record CreateFileDownloadUrlResponse(string downloadUrl);

    public class CreateFileDownloadUrlEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/files/{fileId}/download", async (Guid fileId, ISender sender, HttpContext context) =>
            {
                var result = await sender.Send(new CreateFileDownloadUrlCommand(fileId));

                return Results.Ok(new CreateFileDownloadUrlResponse(result.Url));
            })
            .WithName("CreateFileDownloadUrl")
            .Produces<CreateFileDownloadUrlResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create File Download Url")
            .WithDescription("Create File Download Url");
        }
    }
}

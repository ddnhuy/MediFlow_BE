using FileStorage.API.Queries;

namespace FileStorage.API.Endpoints
{
    public record GetFileByIdResponse(FileMetaDataDto FileMetaData);

    public class GetFileByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/files/{fileId}", async (Guid fileId, ISender sender) =>
            {
                var result = await sender.Send(new GetFileByIdQuery(fileId));

                return Results.Ok(new GetFileByIdResponse(result.FileMetadata));
            })
            .WithName("GetFileById")
            .Produces<GetFileByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get File")
            .WithDescription("Get File By Id");
        }
    }
}

using BuildingBlocks.Strings.Enums;
using FileStorage.API.Queries;

namespace FileStorage.API.Endpoints
{
    public record GetFilesRequest(string? Department = null, string? Type = null);
    public record GetFilesResponse(IEnumerable<FileMetaDataSummaryDto> Files);

    public class GetFilesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/files", async ([AsParameters] GetFilesRequest request, ISender sender) =>
            {
                if (string.IsNullOrEmpty(request.Type))
                {
                    var result = await sender.Send(new GetFilesQuery(request.Department, null));

                    return Results.Ok(new GetFilesResponse(result.Files));
                }

                if (!Enum.TryParse<FileType>(request.Type, true, out var fileType))
                {
                    throw new BadRequestException(ExceptionKey.INVALID_FILE_TYPE);
                }

                var resultWithFileType = await sender.Send(new GetFilesQuery(request.Department, fileType));

                return Results.Ok(new GetFilesResponse(resultWithFileType.Files));
            })
            .WithName("GetFiles")
            .Produces<GetFilesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Files")
            .WithDescription("Get Files");
        }
    }
}

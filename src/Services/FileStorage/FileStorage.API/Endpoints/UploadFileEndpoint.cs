using BuildingBlocks.Strings.Enums;

namespace FileStorage.API.Endpoints
{
    public record UploadFileResponse(FileMetaDataDto FileMetaData);

    public class UploadFileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/files/upload", async (ISender sender, HttpContext context) =>
            {
                IFormCollection form = await context.Request.ReadFormAsync();
                IFormFile? file = form.Files.Count > 0 ? form.Files["file"] : null;

                if (file is null)
                {
                    throw new BadRequestException("File not provided or is empty.");
                }

                string department = form["department"].ToString();
                string typeString = form["type"].ToString();

                if (!Enum.TryParse<FileType>(typeString, true, out var fileType))
                {
                    throw new BadRequestException($"Invalid file type: {typeString}");
                }

                var result = await sender.Send(new UploadFileCommand(file, department, fileType));

                return Results.Ok(result.Adapt<UploadFileResponse>());
            })
            .WithName("UploadFile")
            .Produces<UploadFileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload File")
            .WithDescription("Upload File");
        }
    }
}

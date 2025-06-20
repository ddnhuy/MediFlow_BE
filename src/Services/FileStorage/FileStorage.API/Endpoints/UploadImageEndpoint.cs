using Microsoft.AspNetCore.Mvc;

namespace FileStorage.API.Endpoints
{
    public record UploadImageResponse(string ImageUrl);

    public class UploadImageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/images/upload", async (ISender sender, HttpContext context) =>
            {
                IFormCollection form = await context.Request.ReadFormAsync();
                IFormFile? file = form.Files.Count > 0 ? form.Files["image"] : null;

                if (file is null)
                {
                    throw new BadRequestException(ExceptionKey.FILE_NOT_PROVIDED);
                }

                string? folder = form["folder"].ToString();
                string? imageUrl = form["imageUrl"].ToString();

                var result = await sender.Send(new UploadImageCommand(file, folder, imageUrl));

                return Results.Ok(result.Adapt<UploadImageResponse>());
            })
            .WithName("UploadImage")
            .Produces<UploadImageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload Image")
            .WithDescription("Upload Image");
        }
    }
}

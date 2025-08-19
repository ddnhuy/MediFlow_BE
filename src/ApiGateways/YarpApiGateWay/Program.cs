using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using YarpApiGateWay;
using YarpApiGateWay.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddServices(builder.Configuration)
    .AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseServices();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GetUserInfoMiddleware>();
app.UseMiddleware<PermissionCheckMiddleware>();

// Middleware: Wrap response from services to BaseResponse
app.Use(async (context, next) =>
{
    // Check if this is a file download endpoint
    var isFileDownload = IsFileDownloadEndpoint(context.Request.Path, context.Response.ContentType);

    if (isFileDownload)
    {
        // For file downloads, don't wrap in BaseResponse - pass through directly
        await next();
        return;
    }

    // Capture original response for JSON endpoints
    var originalBodyStream = context.Response.Body;
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;

    await next(); // Forward to YARP

    // Check content type after response to confirm it's not a file
    var contentType = context.Response.ContentType ?? "";
    if (IsFileContentType(contentType))
    {
        // This is actually a file, pass through without JSON wrapping
        context.Response.Body = originalBodyStream;
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        return;
    }

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var rawBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
    context.Response.Body.Seek(0, SeekOrigin.Begin);

    // Build BaseResponse for JSON endpoints only
    var isSuccessStatusCode = context.Response.StatusCode is >= 200 and < 300;
    var messageKey = isSuccessStatusCode
        ? "SUCCESS"
        : (!string.IsNullOrWhiteSpace(rawBody)
            ? JsonSerializer.Deserialize<ProblemDetails>(rawBody)?.Detail ?? "ERROR"
            : "ERROR");

    object? data = null;
    if (isSuccessStatusCode && context.Response.StatusCode != 204 && !string.IsNullOrWhiteSpace(rawBody))
    {
        try
        {
            using var doc = JsonDocument.Parse(rawBody);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.EnumerateObject().Count() == 1)
            {
                var onlyProperty = root.EnumerateObject().First();
                data = JsonSerializer.Deserialize<object>(onlyProperty.Value.GetRawText());
            }
            else
            {
                data = JsonSerializer.Deserialize<object>(rawBody);
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, treat as raw data
            data = rawBody;
        }
    }

    var baseResponse = new BaseResponse<object>
    {
        StatusCode = context.Response.StatusCode,
        MessageKey = messageKey!,
        Data = data
    };

    // Write back BaseResponse
    context.Response.ContentType = "application/json";
    context.Response.Body = originalBodyStream;

    var serialized = JsonSerializer.Serialize(baseResponse);
    context.Response.ContentLength = Encoding.UTF8.GetByteCount(serialized);
    await context.Response.WriteAsync(serialized);
});

app.Run();

static bool IsFileDownloadEndpoint(PathString path, string? contentType = null)
{
    var pathValue = path.Value?.ToLower() ?? "";
    return pathValue.Contains("/export") ||
           pathValue.Contains("/download") ||
           pathValue.EndsWith(".xlsx") ||
           pathValue.EndsWith(".pdf") ||
           pathValue.EndsWith(".csv") ||
           pathValue.EndsWith(".docx");
}

static bool IsFileContentType(string contentType)
{
    var lowerContentType = contentType.ToLower();
    return lowerContentType.Contains("application/vnd.openxmlformats-officedocument") ||
           lowerContentType.Contains("application/octet-stream") ||
           lowerContentType.Contains("application/pdf") ||
           lowerContentType.Contains("image/") ||
           lowerContentType.Contains("text/csv") ||
           (lowerContentType.StartsWith("application/") &&
            !lowerContentType.Contains("json") &&
            !lowerContentType.Contains("xml"));
}

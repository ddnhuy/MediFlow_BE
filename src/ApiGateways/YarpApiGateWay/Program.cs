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
    // Capture original response
    var originalBodyStream = context.Response.Body;
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;

    await next(); // Forward to YARP

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var rawBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
    context.Response.Body.Seek(0, SeekOrigin.Begin);

    // Build BaseResponse
    var isSuccessStatusCode = context.Response.StatusCode is >= 200 and < 300;
    var messageKey = isSuccessStatusCode
        ? "SUCCESS"
        : (!string.IsNullOrWhiteSpace(rawBody)
            ? JsonSerializer.Deserialize<ProblemDetails>(rawBody)?.Detail ?? "ERROR"
            : "ERROR");

    object? data = null;
    if (isSuccessStatusCode && context.Response.StatusCode != 204 && !string.IsNullOrWhiteSpace(rawBody))
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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Threading.RateLimiting;
using YarpApiGateWay;
using YarpApiGateWay.RateLimitOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Get token from cookie
            var accessToken = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var myOptions = new CustomRateLimitOptions();
builder.Configuration.GetSection(CustomRateLimitOptions.MyRateLimit).Bind(myOptions);
var slidingPolicy = "sliding";

builder.Services.AddRateLimiter(_ => _
    .AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
    {
        options.PermitLimit = myOptions.PermitLimit;
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = myOptions.QueueLimit;
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Authorization = $"Bearer {token}";
    }

    await next();
});

// Middleware: Wrap response thành BaseResponse
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
    var message = context.Response.StatusCode == 200
        ? "Success"
        : (!string.IsNullOrWhiteSpace(rawBody) ? JsonSerializer.Deserialize<ProblemDetails>(rawBody)?.Detail : "Error");
    var data = context.Response.StatusCode == 200 && !string.IsNullOrWhiteSpace(rawBody)
        ? JsonSerializer.Deserialize<object>(rawBody)
        : null;

    var baseResponse = new BaseResponse<object>
    {
        StatusCode = context.Response.StatusCode,
        Message = message!,
        Data = data
    };

    // Write back BaseResponse
    context.Response.ContentType = "application/json";
    context.Response.Body = originalBodyStream;
    await context.Response.WriteAsync(JsonSerializer.Serialize(baseResponse));
});

// Route test
static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");
app.MapGet("/", () => Results.Ok($"Sliding Window Limiter {GetTicks()}")).RequireRateLimiting(slidingPolicy);

app.MapReverseProxy();
app.Run();

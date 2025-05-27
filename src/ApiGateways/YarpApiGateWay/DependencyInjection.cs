using HumanResource.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using YarpApiGateWay.RateLimitOptions;
using YarpApiGateWay.Services;

namespace YarpApiGateWay
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // gRPC Services
            services.AddGrpcClient<PolicyProtoService.PolicyProtoServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcSettings:HumanResourceUrl"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return handler;
            });

            // Redis Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Authorization";
            });

            // Gateway Services
            services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"));

            var myOptions = new CustomRateLimitOptions();
            configuration.GetSection(CustomRateLimitOptions.MyRateLimit).Bind(myOptions);

            services.AddRateLimiter(_ => _.AddSlidingWindowLimiter(policyName: "sliding", options =>
            {
                options.PermitLimit = myOptions.PermitLimit;
                options.Window = TimeSpan.FromSeconds(myOptions.Window);
                options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = myOptions.QueueLimit;
            }));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
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

            // Other Services
            services.AddSingleton<IPermissionService, PermissionService>();

            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowCredentials()
                          .WithOrigins("http://localhost:3000", "https://mediflow-cvs.netlify.app/")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseServices(this WebApplication app)
        {
            app.UseRouting();
            app.UseCors();

            // Route test
            static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");
            app.MapGet("/", () => Results.Ok($"Sliding Window Limiter {GetTicks()}")).RequireRateLimiting("sliding");

            app.MapReverseProxy();

            return app;
        }
    }
}

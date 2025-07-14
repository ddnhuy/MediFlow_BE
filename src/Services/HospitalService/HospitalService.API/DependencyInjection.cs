using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Strings.Extensions;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace HospitalService.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCarter();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)
                            ),
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            services.AddAuthorization();

            services.AddExceptionHandler<CustomExceptionHandler>();

            services.AddSeqLogging(serviceName: Assembly.GetExecutingAssembly().GetName().Name!);

            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            app.UseExceptionHandler(options => { });
            app.MapCarter();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}

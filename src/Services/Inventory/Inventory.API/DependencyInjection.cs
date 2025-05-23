namespace Inventory.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCarter();
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthorization(options =>
            {
                AuthorizationPolicies.RegisterPolicies(options);
            });
            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            app.MapCarter();
            app.UseExceptionHandler(options => { });
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

    }
}

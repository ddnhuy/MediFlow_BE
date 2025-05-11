using Infrastructure.Database;

namespace HumanResource.Grpc.Database
{
    public static class Extensions
    {
        public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            if (environment.IsDevelopment())
            {
                // Seed user & roles
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                await SeedData.InitializeUserData(userManager, roleManager);

                // Seed departments
                await SeedData.InitializeDepartmentData(dbContext);
            }

            return app;
        }
    }
}
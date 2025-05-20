namespace Inventory.Infrastructure.Data.Extensions
{
    public static class DatabaseExtension
    {
        public static async Task SeedWarehouseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedAsync(context);        
        }

        public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
            return app;
        }

        private static Warehouse CreateNewWarehouse()
        {
            return new Warehouse
            {
                WarehouseCode = "WH-001",
                WarehouseName = "Main Warehouse",
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
        }

        private static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Warehouses.AnyAsync())
            {
                var warehouse = CreateNewWarehouse();
                await context.Warehouses.AddAsync(warehouse);
                await context.SaveChangesAsync();
            }
        }
    }
}
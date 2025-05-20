namespace Inventory.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Warehouse> Warehouses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

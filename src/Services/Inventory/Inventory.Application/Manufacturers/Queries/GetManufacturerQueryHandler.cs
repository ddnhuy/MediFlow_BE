namespace Inventory.Application.Manufacturers.Queries
{
    public class GetManufacturerQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetManufacturersQuery, GetManufacturersResult>
    {
        public async Task<GetManufacturersResult> Handle(GetManufacturersQuery request, CancellationToken cancellationToken)
        {
            var manufacturers = await dbContext.Manufacturers
                .Where(m => !m.IsCancelled)
                .OrderBy(m => m.ManufacturerName)
                .ProjectToType<ManufacturerDTO>()
                .ToListAsync(cancellationToken);

            return new GetManufacturersResult(manufacturers);
        }
    }
}

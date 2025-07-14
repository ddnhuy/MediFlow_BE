namespace Inventory.Application.Manufacturers.Queries
{
    public record GetManufacturersQuery() : IQuery<GetManufacturersResult>;
    public record GetManufacturersResult(List<ManufacturerDTO> Manufacturers);
}

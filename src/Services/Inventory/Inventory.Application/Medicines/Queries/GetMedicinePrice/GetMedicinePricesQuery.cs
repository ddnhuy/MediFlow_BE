namespace Inventory.Application.Medicines.Queries.GetMedicinePrice
{
    public record GetMedicinePricesQuery(PaginationRequest PaginationRequest) : IQuery<GetMedicinePricesResult>;
    public record GetMedicinePricesResult(PaginatedResult<MedicinePriceDTO> MedicinePrices);
}

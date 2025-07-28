namespace Inventory.Application.Medicines.Queries.GetMedicinePriceById
{
    public record GetMedicinePriceByIdQuery(int Id) : IQuery<GetMedicinePriceByIdResult>;
    public record GetMedicinePriceByIdResult(MedicinePriceDTO MedicinePrice);
}

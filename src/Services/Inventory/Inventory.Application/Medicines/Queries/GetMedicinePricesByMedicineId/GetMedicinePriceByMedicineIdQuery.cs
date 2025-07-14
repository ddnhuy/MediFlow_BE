namespace Inventory.Application.Medicines.Queries.GetMedicinePricesByMedicineId
{
    public record GetMedicinePricesByMedicineIdQuery(int MedicineId) : IQuery<GetMedicinePricesByMedicineIdResult>;
    public record GetMedicinePricesByMedicineIdResult(MedicinePriceDTO MedicinePrices);
}

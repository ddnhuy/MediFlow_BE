namespace Inventory.Application.Medicines.Queries.GetMedicineById
{
    public record GetMedicineByIdQuery(int Id) : IQuery<GetMedicineByIdResult>;
    public record GetMedicineByIdResult(MedicineDTO Medicine);
}

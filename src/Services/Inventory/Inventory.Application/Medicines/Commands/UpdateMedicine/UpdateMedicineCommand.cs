namespace Inventory.Application.Medicines.Commands.UpdateMedicine
{
    public record UpdateMedicineCommand(
        int Id,
        string MedicineCode,
        string MedicineName,
        string Unit,
        string Manufacturer,
        string ActiveIngredient,
        string UsageInstructions,
        string Concentration,
        string Indications,
        string MedicineClassification,
        string RouteOfAdministration,
        string NationalMedicineCode,
        string Description,
        string Note,
        string RegistrationNumber,
        int MedicineTypeId,
        int VaccineTypeId,
        bool IsSuspended,
        bool IsCancelled) : ICommand<UpdateMedicineResult>;

    public record UpdateMedicineResult(bool IsSuccess);
}

using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Commands.CreateMedicine
{
    public record CreateMedicineCommand(
       string MedicineCode,
       string MedicineName,
       string Unit,
       bool? IsRequiredTestingBeforeUse,
       string ActiveIngredient,
       string UsageInstructions,
       string Concentration,
       string Indications,
       string MedicineClassification,
       RouteOfAdministration RouteOfAdministration,
       string NationalMedicineCode,
       string Description,
       string Note,
       string RegistrationNumber,
       int MedicineTypeId,
       int VaccineTypeId) : ICommand<CreateMedicineResult>;

    public record CreateMedicineResult(int Id);
}

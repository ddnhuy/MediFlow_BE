namespace Inventory.Application.Medicines.Commands.DeleteMedicine
{
    public class DeleteMedicineCommandValidator : AbstractValidator<DeleteMedicineCommand>
    {
        public DeleteMedicineCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Vui lòng nhập Id");
        }
    }
}

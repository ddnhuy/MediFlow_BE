namespace Inventory.Application.Exceptions
{
    public class MedicineInteractionExistValidation : BadRequestException
    {
        public MedicineInteractionExistValidation(string message) : base(message)
        {
        }
    }
}

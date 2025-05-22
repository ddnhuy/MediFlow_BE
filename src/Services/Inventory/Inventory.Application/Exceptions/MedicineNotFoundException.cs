namespace Inventory.Application.Exceptions
{
    public class MedicineNotFoundException : NotFoundException
    {
        public MedicineNotFoundException(string message) : base(message)
        {

        }

        public MedicineNotFoundException(string name, object key) : base(name, key)
        {

        }
    }
}

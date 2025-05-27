namespace Inventory.Application.Exceptions
{
    public class SupplierNotFoundException : NotFoundException
    {
        public SupplierNotFoundException(string message) : base(message)
        {

        }

        public SupplierNotFoundException(string name, object key) : base(name, key)
        {

        }
    }
}

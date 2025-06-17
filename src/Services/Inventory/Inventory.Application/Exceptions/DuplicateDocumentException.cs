namespace Inventory.Application.Exceptions
{
    public class DuplicateDocumentException : BadRequestException
    {
        public DuplicateDocumentException(string message) : base(message)
        {
            
        }
    }
}

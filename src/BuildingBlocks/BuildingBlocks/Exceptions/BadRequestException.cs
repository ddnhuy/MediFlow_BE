using BuildingBlocks.Strings;

namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(ExceptionKey key) : base(key.ToString()) { }

        public BadRequestException(ExceptionKey key, string details) : base(key.ToString())
        {
            Details = details;
        }

        public string? Details { get; private set; }
    }
}

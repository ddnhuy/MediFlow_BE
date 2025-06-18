using BuildingBlocks.Strings;

namespace BuildingBlocks.Exceptions
{
    public class InternalServerException : Exception
    {
        public InternalServerException(ExceptionKey key) : base(key.ToString()) { }

        public InternalServerException(ExceptionKey key, string details) : base(key.ToString())
        {
            Details = details;
        }

        public string? Details { get; private set; }
    }
}

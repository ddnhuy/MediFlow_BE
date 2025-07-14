using BuildingBlocks.Strings;

namespace BuildingBlocks.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(ExceptionKey key) : base(key.ToString()) { }

        public NotFoundException(ExceptionKey key, object obj) : base($"Entity \"{key.ToString()}\" ({obj}) was not found.") { }
    }
}

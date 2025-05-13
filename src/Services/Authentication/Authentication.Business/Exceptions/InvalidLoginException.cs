using BuildingBlocks.Exceptions;

namespace Authentication.Business.Exceptions
{
    public class InvalidLoginException : BadRequestException
    {
        public InvalidLoginException(string message) : base("Đăng nhập thất bại", message) { }
    }
}

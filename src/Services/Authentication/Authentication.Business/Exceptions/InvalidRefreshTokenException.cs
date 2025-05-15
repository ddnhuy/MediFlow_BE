using BuildingBlocks.Exceptions;

namespace Authentication.Business.Exceptions
{
    public class InvalidRefreshTokenException : BadRequestException
    {
        public InvalidRefreshTokenException() : base("Đăng nhập thất bại", "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.") { }
    }
}

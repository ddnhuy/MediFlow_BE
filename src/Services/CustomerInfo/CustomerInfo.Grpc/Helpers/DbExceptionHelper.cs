using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.Grpc.Helpers
{
    public static class DbExceptionHelper
    {
        public static bool IsDuplicateKeyException(DbUpdateException exception)
        {
            return exception.InnerException?.Message?.Contains("duplicate key value") == true;
        }
    }
}
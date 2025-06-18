using System.Security.Claims;

namespace Appointment.API.Helpers
{
    public interface ICurrentUserHelper
    {
        int GetUserId();
    }

    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
                             ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException(ExceptionKey.REQUIRED_USER_ID.ToString());
            }

            return userId;
        }
    }
}

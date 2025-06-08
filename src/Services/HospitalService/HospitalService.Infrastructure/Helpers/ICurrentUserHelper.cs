using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Helpers
{
    public interface ICurrentUserHelper
    {
        int UserId { get; }
    }

    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly ILogger<CurrentUserHelper> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHelper(ILogger<CurrentUserHelper> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var id))
                {
                    return id;
                }

                _logger.LogWarning("Unable to retrieve valid UserId from claims.");
                return 0;
            }
        }
    }
}

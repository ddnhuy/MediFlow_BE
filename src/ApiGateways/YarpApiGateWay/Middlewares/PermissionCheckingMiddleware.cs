using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using YarpApiGateWay.Services;

namespace YarpApiGateWay.Middlewares
{
    public class PermissionCheckMiddleware
    {
        private readonly List<string> _allowedAnonymousResourceType = ["authentication"];

        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionCheckMiddleware> _logger;
        private readonly IPermissionService _permissionService;

        public PermissionCheckMiddleware(RequestDelegate next, ILogger<PermissionCheckMiddleware> logger, IPermissionService permissionService)
        {
            _next = next;
            _logger = logger;
            _permissionService = permissionService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var action = context.Request.Method switch
            {
                "GET" => "read",
                _ => "write"
            };

            var path = context.Request.Path.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(path) && path.Contains("health"))
            {
                await _next(context);
                return;
            }

            var segments = path.Trim('/').Split('/');
            var resourceType = segments.Length > 0 ? segments[0] : "unknown";


            if (_allowedAnonymousResourceType.Contains(resourceType))
            {
                await _next(context);
                return;
            }

            // Check JWT
            var authenticateResult = await context.AuthenticateAsync();
            if (!authenticateResult.Succeeded || !context.User.Identity?.IsAuthenticated == true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Check permissions
            var user = context.User;
            var role = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            var department = user.Claims.FirstOrDefault(claim => claim.Type == "department")?.Value;

            if (role is null || department is null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Missing role or department");
                return;
            }

            var permission = await _permissionService.GetPermissionsAsync(role, department, resourceType);

            if (!permission.ToLower().Contains(action.ToLower()))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Missing permission");
                return;
            }

            await _next(context);
        }
    }
}

using HumanResource.Grpc;
using Microsoft.Extensions.Caching.Distributed;

namespace YarpApiGateWay.Services
{
    public interface IPermissionService
    {
        Task<string> GetPermissionsAsync(string role, string department, string resourceType);
    }

    public class PermissionService(
        IDistributedCache cache,
        PolicyProtoService.PolicyProtoServiceClient policyProto
        )
        : IPermissionService
    {
        public async Task<string> GetPermissionsAsync(string role, string department, string resourceType)
        {
            var cacheKey = $"{department}.{role}.{resourceType}";

            var cachedPermissions = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedPermissions))
            {
                return cachedPermissions;
            }

            var result = await policyProto.CheckIfHasPermissionAsync(new CheckIfHasPermissionRequest
            {
                RoleName = role,
                DepartmentNameInEnglish = department.Replace('_', ' '),
                ResourceType = resourceType
            });

            if (result.HasPermission)
            {
                cache.SetString(cacheKey, result.Actions);

                return result.Actions;
            }

            return string.Empty;
        }
    }
}

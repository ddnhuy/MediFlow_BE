using BuildingBlocks.Strings;
using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string Examination = "Examination";

        public static void RegisterPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(Examination, policy =>
            {
                policy.RequireRole([Roles.DOCTOR]);
                policy.RequireClaim("Department", ["Examination"]);
            });
        }
    }
}

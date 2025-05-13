using BuildingBlocks.Strings;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BuildingBlocks.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string CanViewPatientInfo = "CanViewPatientInfo";

        public static void RegisterPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(CanViewPatientInfo, policy => policy.RequireClaim(ClaimTypes.Role, [ Roles.ADMIN, Roles.DOCTOR ]));
        }
    }
}

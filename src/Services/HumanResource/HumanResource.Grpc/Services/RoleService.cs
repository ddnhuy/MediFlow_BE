namespace HumanResource.Grpc.Services
{
    public class RoleService(
        RoleManager<IdentityRole<int>> roleManager) : RoleProtoService.RoleProtoServiceBase
    {
        public override async Task<ListRoleNamesResponse> ListRoleNames(ListRoleNamesRequest request, ServerCallContext context)
        {
            var roleNames = await roleManager.Roles
                .Select(role => role.Name)
                .ToListAsync(context.CancellationToken);

            return new ListRoleNamesResponse
            {
                RoleNames = { roleNames }
            };
        }
    }
}

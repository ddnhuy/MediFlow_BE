namespace Management.API.Roles.Queries
{
    public record GetRolesResult(IEnumerable<string> RoleNames);
    public record GetRoleNamesQuery() : IQuery<GetRolesResult>;

    internal class GetRolesQueryHandler(
        RoleProtoService.RoleProtoServiceClient roleProto) : IQueryHandler<GetRoleNamesQuery, GetRolesResult>
    {
        public async Task<GetRolesResult> Handle(GetRoleNamesQuery query, CancellationToken cancellationToken)
        {
            var result = await roleProto.ListRoleNamesAsync(new ListRoleNamesRequest(), cancellationToken: cancellationToken);

            if (result == null || result.RoleNames.Count == 0)
            {
                return new GetRolesResult(new List<string>());
            }

            return new GetRolesResult(result.RoleNames.ToList());
        }
    }
}

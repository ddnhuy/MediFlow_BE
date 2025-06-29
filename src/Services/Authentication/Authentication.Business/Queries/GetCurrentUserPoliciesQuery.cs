namespace Authentication.Business.Queries
{
    public record GetCurrentUserPoliciesResult(IEnumerable<string> Roles, IEnumerable<string> Departments, IDictionary<string, string> ResourceTypes);
    public record GetCurrentUserPoliciesQuery(string Roles, string Departments) : IQuery<GetCurrentUserPoliciesResult>;

    internal class GetCurrentUserPoliciesQueryHandler(
        PolicyProtoService.PolicyProtoServiceClient policyProto) : IQueryHandler<GetCurrentUserPoliciesQuery, GetCurrentUserPoliciesResult>
    {
        public async Task<GetCurrentUserPoliciesResult> Handle(GetCurrentUserPoliciesQuery query, CancellationToken cancellationToken)
        {
            var result = await policyProto.GetPoliciesByRolesAndDepartmentsAsync(new GetPoliciesByRolesAndDepartmentsRequest
            {
                Roles = query.Roles,
                Departments = query.Departments
            }, cancellationToken: cancellationToken);

            Dictionary<string, string> resourceTypes = new();
            for (int i = 0; i < result.ResourceTypes.Strings.Count; i += 2)
            {
                resourceTypes.Add(result.ResourceTypes.Strings[i], result.Actions.Strings[i]);
            }

            return new GetCurrentUserPoliciesResult(
                result.Roles.Split(','),
                result.Departments.Split(','),
                resourceTypes);
        }
    }
}

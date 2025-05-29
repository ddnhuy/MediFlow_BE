using Management.API.Helpers;

namespace Management.API.Users.Queries
{
    public record GetUsersResult(PaginatedResult<ApplicationUserSummaryModel> Users);
    public record GetUsersQuery(int PageIndex, int PageSize, string? Keyword, string Roles) : IQuery<GetUsersResult>;

    internal class GetUsersQueryHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : IQueryHandler<GetUsersQuery, GetUsersResult>
    {
        public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: query.Roles);

            var result = await applicationUserProto.ListApplicationUsersAsync(new ListApplicationUsersRequest
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Keyword = query.Keyword ?? string.Empty
            }, metadata, cancellationToken: cancellationToken);

            return new GetUsersResult(
                new PaginatedResult<ApplicationUserSummaryModel>(
                    result.PageIndex,
                    result.PageSize,
                    result.Count,
                    result.Data));
        }
    }
}

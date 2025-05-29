using Management.API.Dtos.User;
using Management.API.Helpers;

namespace Management.API.Users.Queries
{
    public record GetUsersResult(PaginatedResult<UserSummaryDto> Users);
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

            var data = result.Data.Select(user => new UserSummaryDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Code = user.Code,
                Name = user.Name,
                IsSuspended = user.IsSuspended,
                Roles = user.Roles.Split(',').ToList(),
                ProfilePictureUrl = user.ProfilePictureUrl
            }).ToList();

            return new GetUsersResult(
                new PaginatedResult<UserSummaryDto>(
                    result.PageIndex,
                    result.PageSize,
                    result.Count,
                    data));
        }
    }
}

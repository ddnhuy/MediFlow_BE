using BuildingBlocks.Strings;
using Grpc.Core;

namespace Authentication.Business.Queries
{
    public record GetCurrentUserResult(int Id, string Code, string UserName, string Email, string Name, string? ProfilePictureUrl, string Roles);
    public record GetCurrentUserQuery(int UserId, string Roles) : IQuery<GetCurrentUserResult>;

    internal class GetCurrentUserQueryHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : IQueryHandler<GetCurrentUserQuery, GetCurrentUserResult>
    {
        public async Task<GetCurrentUserResult> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var metadata = new Metadata();
            metadata.Add("x-roles", query.Roles);

            var result = await applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = query.UserId
            }, metadata, cancellationToken: cancellationToken);

            return result.Adapt<GetCurrentUserResult>();
        }
    }
}

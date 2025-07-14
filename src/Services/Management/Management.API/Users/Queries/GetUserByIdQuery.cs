using Management.API.Dtos.User;
using Management.API.Helpers;

namespace Management.API.Users.Queries
{
    public record GetUserByIdResult(UserDetailDto User);
    public record GetUserByIdQuery(int UserId, string Roles) : IQuery<GetUserByIdResult>;

    internal class GetUserByIdQueryHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
    {
        public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: query.Roles);

            var result = await applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = query.UserId
            }, metadata, cancellationToken: cancellationToken);

            return new GetUserByIdResult(ConvertUserHelper.ToUserDetailDto(result));
        }
    }
}

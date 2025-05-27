namespace Authentication.Business.Queries
{
    public record GetCurrentUserResult(int Id, string Code, string UserName, string Email, string Name, string? ProfilePictureUrl, string Roles);
    public record GetCurrentUserQuery(int UserId) : IQuery<GetCurrentUserResult>;

    internal class GetCurrentUserQueryHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : IQueryHandler<GetCurrentUserQuery, GetCurrentUserResult>
    {
        public async Task<GetCurrentUserResult> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var result = await applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest { Id = query.UserId }, cancellationToken: cancellationToken);

            return result.Adapt<GetCurrentUserResult>();
        }
    }
}

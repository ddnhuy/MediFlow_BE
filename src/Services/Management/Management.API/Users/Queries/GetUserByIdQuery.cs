using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
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

            return new GetUserByIdResult(new UserDetailDto
            {
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                Code = result.Code,
                Name = result.Name,
                PhoneNumber = result.PhoneNumber,
                EmailConfirmed = result.EmailConfirmed,
                PhoneNumberConfirmed = result.PhoneNumberConfirmed,
                TwoFactorEnabled = result.TwoFactorEnabled,
                IsSuspended = result.IsSuspended,
                IsCancelled = result.IsCancelled,
                CreatedAt = result.CreatedAt.ToDateTime(),
                LastUpdatedAt = result.LastUpdatedAt.ToDateTime(),
                Roles = result.Roles.Split(',').ToList(),
                Departments = result.Departments.Select(d => new DepartmentSummaryDto
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    NameInEnglish = d.NameInEnglish,
                    DepartmentType = new DepartmentTypeSummaryDto
                    {
                        Name = d.DepartmentTypeName,
                        NameInEnglish = d.DepartmentTypeNameInEnglish
                    },
                    IsSuspended = d.IsSuspended
                }).ToList(),
                Address = result.Address,
                ProfilePictureUrl = result.ProfilePictureUrl
            });
        }
    }
}

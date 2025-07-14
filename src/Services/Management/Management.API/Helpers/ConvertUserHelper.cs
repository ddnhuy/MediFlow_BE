using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
using Management.API.Dtos.User;

namespace Management.API.Helpers
{
    public static class ConvertUserHelper
    {
        public static UserDetailDto ToUserDetailDto(ApplicationUserDetailModel from)
        {
            return new UserDetailDto
            {
                Id = from.Id,
                UserName = from.UserName,
                Email = from.Email,
                Code = from.Code,
                Name = from.Name,
                PhoneNumber = from.PhoneNumber,
                EmailConfirmed = from.EmailConfirmed,
                PhoneNumberConfirmed = from.PhoneNumberConfirmed,
                TwoFactorEnabled = from.TwoFactorEnabled,
                IsSuspended = from.IsSuspended,
                IsCancelled = from.IsCancelled,
                CreatedAt = from.CreatedAt.ToDateTime(),
                LastUpdatedAt = from.LastUpdatedAt.ToDateTime(),
                Roles = from.Roles.Split(',').ToList(),
                Departments = from.Departments.Select(d => new DepartmentSummaryDto
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
                Address = from.Address,
                ProfilePictureUrl = from.ProfilePictureUrl
            };
        }
    }
}
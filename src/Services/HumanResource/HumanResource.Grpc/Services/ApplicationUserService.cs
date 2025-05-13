using Grpc.Core;
using HumanResource.Grpc.Database;

namespace HumanResource.Grpc.Services
{
    public class ApplicationUserService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        ILogger<ApplicationUserService> logger)
        : ApplicationUserProtoService.ApplicationUserProtoServiceBase
    {
        public override async Task<ListApplicationUsersResponse> ListApplicationUsers(ListApplicationUsersRequest request, ServerCallContext context)
        {
            logger.LogInformation("Listing application users. Keyword: {Keyword}, Page: {PageIndex}, Size: {PageSize}", request.Keyword, request.PageIndex, request.PageSize);

            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.UserName!.Contains(request.Keyword) || x.Email!.Contains(request.Keyword));
            }

            var totalItems = await query.CountAsync();

            var users = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            logger.LogInformation("Found {Count} users.", totalItems);

            var result = new ListApplicationUsersResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Count = totalItems,
                Data = { users.Adapt<List<ApplicationUserSummaryModel>>() }
            };

            for (int i = 0; i < users.Count; i++)
            {
                var roles = await userManager.GetRolesAsync(users[i]);
                result.Data[i].Roles = string.Join(",", [.. roles]);
            }

            return result;
        }

        public override async Task<ApplicationUserDetailModel> GetApplicationUser(GetApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting user by ID: {Id}", request.Id);

            var user = await dbContext.Users
                .Include(x => x.Departments)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsCancelled);
            if (user == null)
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, $"Không tìm thấy người dùng với ID \"{request.Id}\"."));
            }

            var userModel = user.Adapt<ApplicationUserDetailModel>();
            var roles = await userManager.GetRolesAsync(user);
            userModel.Roles = string.Join(",", [.. roles]);

            logger.LogInformation("Retrieved user: {UserName}", user.UserName);
            return userModel;
        }

        public override async Task<ApplicationUserDetailModel> CreateApplicationUser(CreateApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new user: {UserName} ({Email})", request.UserName, request.Email);

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Code = request.Code,
                Name = request.Name,
                IsSuspended = false,
                IsCancelled = false,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create user {UserName}: {Errors}", request.UserName, string.Join("; ", result.Errors.Select(e => e.Description)));
                throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(", ", result.Errors.Select(x => x.Description))));
            }

            var departments = await dbContext.Departments
                .Where(d => request.DepartmentIds.Contains(d.Id))
                .ToListAsync();

            user.Departments = departments;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("User created successfully: {UserId}", user.Id);
            return user.Adapt<ApplicationUserDetailModel>();
        }

        public override async Task<ApplicationUserDetailModel> UpdateApplicationUser(UpdateApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating user: {Id}", request.Id);

            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, $"Không tìm thấy người dùng với ID \"{request.Id}\"."));
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Name = request.Name;
            user.IsSuspended = request.IsSuspended;
            user.IsCancelled = request.IsCancelled;

            var departments = await dbContext.Departments
                .Where(d => request.DepartmentIds.Contains(d.Id))
                .ToListAsync();

            user.Departments = departments;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to update user {Id}: {Errors}", user.Id, string.Join("; ", result.Errors.Select(e => e.Description)));
                throw new RpcException(new Status(StatusCode.Internal, $"Cập nhật thông tin người dùng với ID \"{request.Id}\" thất bại."));
            }

            logger.LogInformation("User updated successfully: {Id}", user.Id);
            return user.Adapt<ApplicationUserDetailModel>();
        }

        public override async Task<DeleteApplicationUserResponse> DeleteApplicationUser(DeleteApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting user: {Id}", request.Id);

            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, $"Không tìm thấy người dùng với ID \"{request.Id}\""));
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                logger.LogInformation("User deleted: {Id}", user.Id);
            else
                logger.LogWarning("Failed to delete user {Id}", user.Id);

            return new DeleteApplicationUserResponse { Success = result.Succeeded };
        }

        public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            logger.LogInformation("Changing password for user: {Id}", request.UserId);

            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found for password change: {Id}", request.UserId);
                return new ChangePasswordResponse { IsSuccess = false, Message = $"Không tìm thấy người dùng với ID \"{request.UserId}\"" };
            }

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                logger.LogInformation("Password changed for user: {Id}", user.Id);
            else
                logger.LogWarning("Failed to change password for user {Id}: {Errors}", user.Id, string.Join("; ", result.Errors.Select(e => e.Description)));

            return new ChangePasswordResponse
            {
                IsSuccess = result.Succeeded,
                Message = result.Succeeded ? "Mật khẩu đã thay đổi." : "Mật khẩu hiện tại chưa chính xác hoặc mật khẩu mới không hợp lệ, vui lòng thử lại."
            };
        }

        public override async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            logger.LogInformation("Resetting password for email: {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogWarning("User not found for reset: {Email}", request.Email);
                return new ResetPasswordResponse { IsSuccess = false, Message = $"Không tìm thấy người dùng với email \"{request.Email}\"" };
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = PasswordGenerator.GenerateSecurePassword();

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to reset password for user {Email}: {Errors}", user.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
                return new ResetPasswordResponse { IsSuccess = false, Message = "Đặt lại mật khẩu không thành công. Vui lòng thử lại." };
            }

            logger.LogInformation("Password for user {Email} reset to: {Password}", user.Email, newPassword);

            // Send Email

            return new ResetPasswordResponse
            {
                IsSuccess = true,
                Message = $"Đã đặt lại mật khẩu thành công. Vui lòng kiểm tra email của bạn."
            };
        }
        public override async Task<FindApplicationUserByNameResponse> FindApplicationUserByName(FindApplicationUserByNameRequest request, ServerCallContext context)
        {
            logger.LogInformation("Searching for application users with name containing: {Name}", request.Name);

            var users = await userManager.Users
                .Where(u => u.Name.Contains(request.Name))
                .ToListAsync();

            logger.LogInformation("Found {Count} users matching the name {Name}.", users.Count, request.Name);

            var response = new FindApplicationUserByNameResponse();

            foreach (var user in users)
            {
                var userModel = user.Adapt<ApplicationUserSummaryModel>();
                var roles = await userManager.GetRolesAsync(user);
                userModel.Roles = string.Join(",", roles);
                response.Users.Add(userModel);
            }

            return response;
        }

        public override async Task<FindApplicationUserByUserNameResponse> FindApplicationUserByUserName(FindApplicationUserByUserNameRequest request, ServerCallContext context)
        {
            logger.LogInformation("Searching for application users with username: {UserName}", request.UserName);

            var users = await userManager.Users
                .Where(u => u.UserName!.Contains(request.UserName))
                .ToListAsync();

            logger.LogInformation("Found {Count} users matching the username {UserName}.", users.Count, request.UserName);

            var response = new FindApplicationUserByUserNameResponse();

            foreach (var user in users)
            {
                var userModel = user.Adapt<ApplicationUserSummaryModel>();
                var roles = await userManager.GetRolesAsync(user);
                userModel.Roles = string.Join(",", roles);
                response.Users.Add(userModel);
            }

            return response;
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            logger.LogInformation("Login attempt for user: {UserName}", request.UserName);

            var user = await dbContext.Users
                .Include(x => x.Departments)
                .ThenInclude(x => x.DepartmentType)
                .FirstOrDefaultAsync(x => x.UserName == request.UserName && !x.IsCancelled);
            if (user == null)
            {
                logger.LogWarning("User not found: {UserName}", request.UserName);
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Tên người dùng hoặc mật khẩu không chính xác, vui lòng thử lại."
                };
            }

            var result = await userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                logger.LogWarning("Invalid password attempt for user: {UserName}", request.UserName);
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Tên người dùng hoặc mật khẩu không chính xác, vui lòng thử lại."
                };
            }

            var userModel = user.Adapt<ApplicationUserDetailModel>();
            var roles = await userManager.GetRolesAsync(user);
            userModel.Roles = string.Join(",", roles);

            logger.LogInformation("User logged in successfully: {UserName}", user.UserName);

            return new LoginResponse
            {
                IsSuccess = true,
                Message = "Đăng nhập thành công.",
                User = userModel
            };
        }
    }
}
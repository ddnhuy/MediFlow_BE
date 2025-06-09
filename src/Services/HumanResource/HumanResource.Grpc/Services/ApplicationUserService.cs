using BuildingBlocks.Messaging.Contracts.Email;
using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;
using MassTransit;
using MassTransit.Transports;

namespace HumanResource.Grpc.Services
{
    public class ApplicationUserService(
        ICurrentUserHelper currentUserHelper,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        ILogger<ApplicationUserService> logger)
        : ApplicationUserProtoService.ApplicationUserProtoServiceBase
    {
        private int GetUserIdFromContext(ServerCallContext context)
        {
            var userId = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-user-id")?.Value;

            return int.TryParse(userId, out var result) ? result : 0;
        }

        private string GetRolesFromContext(ServerCallContext context)
        {
            return context.RequestHeaders.FirstOrDefault(x => x.Key == "x-roles")!.Value;
        }

        public override async Task<ListApplicationUsersResponse> ListApplicationUsers(ListApplicationUsersRequest request, ServerCallContext context)
        {
            logger.LogInformation("Listing application users. Keyword: {Keyword}, Page: {PageIndex}, Size: {PageSize}", request.Keyword, request.PageIndex, request.PageSize);

            var currentUserRoles = GetRolesFromContext(context);

            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName!.Contains(request.Keyword) || x.Name!.Contains(request.Keyword) || x.Email!.Contains(request.Keyword));
            }

            var rolesToExclude = new List<string>();
            if (currentUserRoles.Contains(Roles.ADMIN))
            {
                rolesToExclude.Add(Roles.ADMIN);
            }
            if (currentUserRoles.Contains(Roles.HEAD_OF_DEPARTMENT))
            {
                rolesToExclude.Add(Roles.ADMIN);
                rolesToExclude.Add(Roles.HEAD_OF_DEPARTMENT);
            }

            if (rolesToExclude.Any())
            {
                var usersList = await query.ToListAsync();

                var userIdsToExclude = new List<int>();
                foreach (var user in usersList)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Any(r => rolesToExclude.Contains(r)))
                    {
                        userIdsToExclude.Add(user.Id);
                    }
                }

                query = query.Where(u => !userIdsToExclude.Contains(u.Id));
            }

            var totalItems = await query.CountAsync();

            var users = await query
                .Where(x => !x.IsCancelled)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .OrderBy(x => x.Code)
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
                result.Data[i].Roles = string.Join(",", roles);
            }

            return result;
        }

        public override async Task<ApplicationUserDetailModel> GetApplicationUser(GetApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting user by ID: {Id}", request.Id);

            var currentUserRoles = GetRolesFromContext(context);

            var user = await dbContext.Users
                .Include(x => x.Departments)
                .ThenInclude(x => x.DepartmentType)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsCancelled);
            if (user == null || (string.Join(",", (await userManager.GetRolesAsync(user))).Contains(Roles.ADMIN) && currentUserRoles != Roles.ADMIN))
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, HumanResourceExceptionStrings.NOT_FOUND_USER_WITH_ID(request.Id)));
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

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Code = request.Code,
                Name = request.Name,
                Address = request.Address,
                ProfilePictureUrl = request.ProfilePictureUrl,
                IsSuspended = false,
                IsCancelled = false,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create user {UserName}: {Errors}", request.UserName, string.Join("; ", result.Errors.Select(e => e.Description)));
                throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(", ", result.Errors.Select(x => x.Description))));
            }

            var roles = request.RoleNames.Distinct().ToList();
            if (roles.Count > 0)
            {
                var roleResult = await userManager.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    logger.LogWarning("Failed to assign roles to user {UserName}: {Errors}", request.UserName, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    throw new RpcException(new Status(StatusCode.Internal, HumanResourceExceptionStrings.FAILED_ASSIGN_ROLE_TO_USER));
                }
            }

            var departments = await dbContext.Departments
                .Where(d => request.DepartmentIds.Contains(d.Id))
                .Include(d => d.DepartmentType)
                .ToListAsync();

            user.Departments = departments;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

            var response = user.Adapt<ApplicationUserDetailModel>();
            response.Roles = string.Join(",", roles);

            return response;
        }

        public override async Task<ApplicationUserDetailModel> UpdateApplicationUser(UpdateApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating user: {Id}", request.Id);

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            var user = await userManager.Users
                .Include(u => u.Departments)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (user is null)
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, HumanResourceExceptionStrings.NOT_FOUND_USER_WITH_ID(request.Id)));
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Code = request.Code;
            user.Name = request.Name;
            user.Address = request.Address;
            user.ProfilePictureUrl = request.ProfilePictureUrl;
            user.IsSuspended = request.IsSuspended;

            // Update Departments
            var departments = await dbContext.Departments
                .Where(d => request.DepartmentIds.Contains(d.Id))
                .Include(d => d.DepartmentType)
                .ToListAsync();
            user.Departments = departments;

            // Update Roles
            var existingRoles = await userManager.GetRolesAsync(user);
            var newRoles = request.RoleNames.Distinct().ToList();

            var rolesToAdd = newRoles.Except(existingRoles).ToList();
            var rolesToRemove = existingRoles.Except(newRoles).ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    logger.LogWarning("Failed to remove roles from user {Id}: {Errors}", user.Id, string.Join("; ", removeResult.Errors.Select(e => e.Description)));
                    throw new RpcException(new Status(StatusCode.Internal, HumanResourceExceptionStrings.FAILED_ASSIGN_ROLE_TO_USER));
                }
            }

            if (rolesToAdd.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    logger.LogWarning("Failed to assign roles to user {Id}: {Errors}", user.Id, string.Join("; ", addResult.Errors.Select(e => e.Description)));
                    throw new RpcException(new Status(StatusCode.Internal, HumanResourceExceptionStrings.FAILED_ASSIGN_ROLE_TO_USER));
                }
            }

            // Update user entity
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                logger.LogWarning("Failed to update user {Id}: {Errors}", user.Id, string.Join("; ", updateResult.Errors.Select(e => e.Description)));
                throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(", ", updateResult.Errors.Select(x => x.Description))));
            }

            await dbContext.SaveChangesAsync();

            logger.LogInformation("User updated successfully: {Id}", user.Id);

            var response = user.Adapt<ApplicationUserDetailModel>();
            response.Roles = string.Join(",", newRoles);

            return response;
        }

        public override async Task<DeleteApplicationUserResponse> DeleteApplicationUser(DeleteApplicationUserRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting user: {Id}", request.Id);

            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user is null)
            {
                logger.LogWarning("User not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, HumanResourceExceptionStrings.NOT_FOUND_USER_WITH_ID(request.Id)));
            }

            user.IsSuspended = true;
            user.IsCancelled = true;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                logger.LogInformation("User deleted: {Id}", user.Id);
            else
                logger.LogWarning("Failed to delete user {Id}", user.Id);

            return new DeleteApplicationUserResponse { IsSuccess = result.Succeeded };
        }

        public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            logger.LogInformation("Changing password for user: {Id}", request.UserId);

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found for password change: {Id}", request.UserId);
                return new ChangePasswordResponse { IsSuccess = false, Message = HumanResourceExceptionStrings.NOT_FOUND_USER_WITH_ID(request.UserId) };
            }

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                logger.LogInformation("Password changed for user: {Id}", user.Id);
            else
                logger.LogWarning("Failed to change password for user {Id}: {Errors}", user.Id, string.Join("; ", result.Errors.Select(e => e.Description)));

            return new ChangePasswordResponse
            {
                IsSuccess = result.Succeeded,
                Message = result.Succeeded ? HumanResourceSuccessStrings.SUCCESS_CHANGE_PASSWORD : HumanResourceExceptionStrings.FAILED_CHANGE_PASSWORD
            };
        }

        public override async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            logger.LogInformation("Resetting password for email: {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogWarning("User not found for reset: {Email}", request.Email);
                return new ResetPasswordResponse { IsSuccess = false, Message = HumanResourceExceptionStrings.NOT_FOUND_USER_WITH_EMAIL(request.Email) };
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = PasswordGenerator.GenerateSecurePassword();

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to reset password for user {Email}: {Errors}", user.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
                return new ResetPasswordResponse { IsSuccess = false, Message = HumanResourceExceptionStrings.FAILED_RESET_PASSWORD };
            }

            logger.LogInformation("Password for user {Email} reset to: {Password}", user.Email, newPassword);

            // Send Email
            await publishEndpoint.Publish(new SendEmailMessage
            {
                To = user.Email!,
                SubjectCode = EmailSubjectCode.ResetPasswordSuccess,
                TemplateData = new Dictionary<string, string>
                {
                    ["FullName"] = user.Name ?? user.Email!,
                    ["ResetTime"] = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    ["NewPassword"] = newPassword,
                }
            }, context.CancellationToken);

            return new ResetPasswordResponse
            {
                IsSuccess = true,
                Message = HumanResourceSuccessStrings.SUCCESS_RESET_PASSWORD
            };
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            logger.LogInformation("Login attempt for user: {UserName}", request.UserName);

            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null || user.IsCancelled)
            {
                logger.LogWarning("Login failed: user not found or cancelled.");
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = HumanResourceExceptionStrings.INVALID_LOGIN_CREDENTIAL
                };
            }

            var result = await userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                logger.LogWarning("Login failed: invalid password for user: {UserName}", request.UserName);
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = HumanResourceExceptionStrings.INVALID_LOGIN_CREDENTIAL
                };
            }

            var fullUser = await dbContext.Users
                .AsNoTracking()
                .Include(x => x.Departments)
                .ThenInclude(x => x.DepartmentType)
                .FirstAsync(x => x.Id == user.Id);

            var userModel = fullUser.Adapt<ApplicationUserDetailModel>();
            var roles = await userManager.GetRolesAsync(user);
            userModel.Roles = string.Join(",", roles);

            logger.LogInformation("User logged in successfully: {UserName}", user.UserName);

            return new LoginResponse
            {
                IsSuccess = true,
                Message = HumanResourceSuccessStrings.SUCCESS_LOGIN,
                User = userModel
            };
        }
    }
}
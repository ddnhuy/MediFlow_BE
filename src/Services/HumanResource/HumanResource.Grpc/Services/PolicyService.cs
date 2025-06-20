namespace HumanResource.Grpc.Services
{
    public class PolicyService(
        ICurrentUserHelper currentUserHelper,
        ApplicationDbContext dbContext,
        ILogger<PolicyService> logger)
        : PolicyProtoService.PolicyProtoServiceBase
    {
        private int GetUserIdFromContext(ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            var claimValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out var userId) ? userId : 0;
        }

        public override async Task<GetPoliciesResponse> GetPolicies(GetPoliciesRequest request, ServerCallContext context)
        {
            logger.LogInformation("Listing policies. Page: {PageIndex}, Size: {PageSize}",
                request.PageIndex, request.PageSize);

            var query = dbContext.Policies.AsQueryable();

            var totalCount = await query.CountAsync();

            var policies = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            logger.LogInformation("Found {Count} policies", totalCount);

            var response = new GetPoliciesResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            foreach (var policy in policies)
            {
                var policyModel = new PolicyModel
                {
                    Id = policy.Id,
                    ResourceType = policy.ResourceType
                };
                policyModel.Actions.AddRange(policy.Actions);

                response.Policies.Add(policyModel);
            }

            return response;
        }

        public override async Task<CheckIfHasPermissionResponse> CheckIfHasPermission(CheckIfHasPermissionRequest request, ServerCallContext context)
        {
            logger.LogInformation("Checking permissions for RoleName: {RoleName}, DepartmentName: {DepartmentName}, ResourceType: {ResourceType}",
                request.RoleName, request.DepartmentNameInEnglish, request.ResourceType);

            var permission = await dbContext.RoleDepartmentPolicies
                .Include(rdp => rdp.Role)
                .Include(rdp => rdp.Department)
                .Include(rdp => rdp.Policy)
                .Where(rdp => rdp.Role.Name! == request.RoleName &&
                            rdp.Department.NameInEnglish == request.DepartmentNameInEnglish &&
                            rdp.Policy.ResourceType == request.ResourceType)
                .ToListAsync();

            var hasPermission = permission.Any();
            var hasWriteAction = permission.Any(p => p.Policy.Actions.Contains("write", StringComparer.OrdinalIgnoreCase));

            logger.LogInformation("Permission check result: {HasPermission}", hasPermission);

            return new CheckIfHasPermissionResponse
            {
                HasPermission = hasPermission,
                Actions = hasPermission ?
                    hasWriteAction ? "read_write" : "read"
                    : string.Empty
            };
        }

        public override async Task<PolicyModel> GetPolicy(GetPolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting policy by ID: {Id}", request.Id);

            var policy = await dbContext.Policies
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (policy == null)
            {
                logger.LogWarning("Policy not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_POLICY_WITH_ID.ToString()));
            }

            logger.LogInformation("Retrieved policy: {PolicyId} with ResourceType: {ResourceType}", policy.Id, policy.ResourceType);

            var policyModel = new PolicyModel
            {
                Id = policy.Id,
                ResourceType = policy.ResourceType
            };
            policyModel.Actions.AddRange(policy.Actions);

            return policyModel;
        }

        public override async Task<PolicyModel> CreatePolicy(CreatePolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new policy with ResourceType: {ResourceType}", request.ResourceType);

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            var policy = new Policy
            {
                ResourceType = request.ResourceType,
                Actions = new List<string>(request.Actions)
            };

            await dbContext.Policies.AddAsync(policy);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Policy created successfully with ID: {Id}", policy.Id);

            var policyModel = new PolicyModel
            {
                Id = policy.Id,
                ResourceType = policy.ResourceType
            };
            policyModel.Actions.AddRange(policy.Actions);

            return policyModel;
        }

        public override async Task<PolicyModel> UpdatePolicy(UpdatePolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating policy: {Id}", request.Id);

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            var policy = await dbContext.Policies
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (policy == null)
            {
                logger.LogWarning("Policy not found: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_POLICY_WITH_ID.ToString()));
            }

            policy.ResourceType = request.ResourceType;
            policy.Actions = new List<string>(request.Actions);

            dbContext.Policies.Update(policy);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Policy updated successfully: {Id}", policy.Id);

            var policyModel = new PolicyModel
            {
                Id = policy.Id,
                ResourceType = policy.ResourceType
            };
            policyModel.Actions.AddRange(policy.Actions);

            return policyModel;
        }

        public override async Task<DeleteResponse> DeletePolicy(DeletePolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting policy: {Id}", request.Id);

            var policy = await dbContext.Policies
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (policy == null)
            {
                logger.LogWarning("Policy not found: {Id}", request.Id);
                return new DeleteResponse
                {
                    Success = false,
                    Message = ExceptionKey.NOT_FOUND_POLICY_WITH_ID.ToString()
                };
            }

            // Check for existing relationships
            var hasRelationships = await dbContext.RoleDepartmentPolicies.AnyAsync(rdp => rdp.PolicyId == request.Id);

            if (hasRelationships)
            {
                logger.LogWarning("Cannot delete policy {Id} because it is associated with role-department assignments", request.Id);
                return new DeleteResponse
                {
                    Success = false,
                    Message = ExceptionKey.CANNOT_DELETE_POLICY_WITH_RELATIONSHIPS.ToString()
                };
            }

            dbContext.Policies.Remove(policy);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Policy deleted successfully: {Id}", request.Id);

            return new DeleteResponse
            {
                Success = true,
                Message = HumanResourceSuccessStrings.SUCCESS_DELETE_POLICY
            };
        }

        public override async Task<GetRoleDepartmentPoliciesResponse> GetRoleDepartmentPolicies(GetRoleDepartmentPoliciesRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting role-department policies. RoleName: {RoleName}, DepartmentName: {DepartmentName}, ResourceType: {ResourceType}, Page: {PageIndex}, Size: {PageSize}",
                request.RoleName, request.DepartmentName, request.ResourceType, request.PageIndex, request.PageSize);

            var query = dbContext.RoleDepartmentPolicies
                .Include(rdp => rdp.Role)
                .Include(rdp => rdp.Department)
                .Include(rdp => rdp.Policy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                query = query.Where(rdp => rdp.Role.Name!.Contains(request.RoleName));
            }

            if (!string.IsNullOrWhiteSpace(request.DepartmentName))
            {
                query = query.Where(rdp => rdp.Department.Name.Contains(request.DepartmentName));
            }

            if (!string.IsNullOrWhiteSpace(request.ResourceType))
            {
                query = query.Where(rdp => rdp.Policy.ResourceType.Contains(request.ResourceType));
            }

            var totalCount = await query.CountAsync();

            var roleDepartmentPolicies = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            logger.LogInformation("Found {Count} role-department policies", totalCount);

            var response = new GetRoleDepartmentPoliciesResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            foreach (var rdp in roleDepartmentPolicies)
            {
                var policyModel = new PolicyModel
                {
                    Id = rdp.Policy.Id,
                    ResourceType = rdp.Policy.ResourceType
                };
                policyModel.Actions.AddRange(rdp.Policy.Actions);

                var roleModel = new RoleModel
                {
                    Id = rdp.Role.Id,
                    Name = rdp.Role.Name
                };

                var departmentModel = new DepartmentModel
                {
                    Id = rdp.Department.Id,
                    Name = rdp.Department.Name,
                    Code = rdp.Department.Code
                };

                var rdpModel = new RoleDepartmentPolicyModel
                {
                    Id = rdp.Id,
                    Role = roleModel,
                    Department = departmentModel,
                    Policy = policyModel
                };

                response.Policies.Add(rdpModel);
            }

            return response;
        }

        public override async Task<RoleDepartmentPolicyModel> AssignPolicyToRoleDepartment(AssignPolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Assigning policy {PolicyId} to role {RoleId} and department {DepartmentId}",
                request.PolicyId, request.RoleId, request.DepartmentId);

            currentUserHelper.SetUserId(GetUserIdFromContext(context));

            // Check if the assignment already exists
            var existingAssignment = await dbContext.RoleDepartmentPolicies
                .FirstOrDefaultAsync(rdp => rdp.RoleId == request.RoleId &&
                                    rdp.DepartmentId == request.DepartmentId &&
                                    rdp.PolicyId == request.PolicyId);

            if (existingAssignment != null)
            {
                logger.LogWarning("Policy {PolicyId} is already assigned to role {RoleId} and department {DepartmentId}",
                    request.PolicyId, request.RoleId, request.DepartmentId);
                throw new RpcException(new Status(StatusCode.AlreadyExists, ExceptionKey.POLICY_ASSIGNMENT_ALREADY_EXISTS.ToString()));
            }

            // Verify role exists
            var role = await dbContext.Roles.FindAsync(request.RoleId);
            if (role == null)
            {
                logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_ROLE_WITH_ID.ToString()));
            }

            // Verify department exists
            var department = await dbContext.Departments.FindAsync(request.DepartmentId);
            if (department == null)
            {
                logger.LogWarning("Department not found: {DepartmentId}", request.DepartmentId);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_DEPARTMENT_WITH_ID.ToString()));
            }

            // Verify policy exists
            var policy = await dbContext.Policies.FindAsync(request.PolicyId);
            if (policy == null)
            {
                logger.LogWarning("Policy not found: {PolicyId}", request.PolicyId);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_POLICY_WITH_ID.ToString()));
            }

            var roleDepartmentPolicy = new RoleDepartmentPolicy
            {
                RoleId = request.RoleId,
                DepartmentId = request.DepartmentId,
                PolicyId = request.PolicyId
            };

            await dbContext.RoleDepartmentPolicies.AddAsync(roleDepartmentPolicy);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Policy assignment created with ID: {Id}", roleDepartmentPolicy.Id);

            // Construct response model with loaded relationships
            await dbContext.Entry(roleDepartmentPolicy).Reference(rdp => rdp.Role).LoadAsync();
            await dbContext.Entry(roleDepartmentPolicy).Reference(rdp => rdp.Department).LoadAsync();
            await dbContext.Entry(roleDepartmentPolicy).Reference(rdp => rdp.Policy).LoadAsync();

            var policyModel = new PolicyModel
            {
                Id = policy.Id,
                ResourceType = policy.ResourceType
            };
            policyModel.Actions.AddRange(policy.Actions);

            var roleModel = new RoleModel
            {
                Id = role.Id,
                Name = role.Name
            };

            var departmentModel = new DepartmentModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code
            };

            return new RoleDepartmentPolicyModel
            {
                Id = roleDepartmentPolicy.Id,
                Role = roleModel,
                Department = departmentModel,
                Policy = policyModel
            };
        }

        public override async Task<DeleteResponse> RevokePolicyFromRoleDepartment(RevokePolicyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Revoking policy assignment: {Id}", request.RoleDepartmentPolicyId);

            var roleDepartmentPolicy = await dbContext.RoleDepartmentPolicies
                .FindAsync(request.RoleDepartmentPolicyId);

            if (roleDepartmentPolicy == null)
            {
                logger.LogWarning("Role-department-policy assignment not found: {Id}", request.RoleDepartmentPolicyId);
                return new DeleteResponse
                {
                    Success = false,
                    Message = ExceptionKey.NOT_FOUND_PERMISSION_WITH_ID.ToString()
                };
            }

            dbContext.RoleDepartmentPolicies.Remove(roleDepartmentPolicy);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Policy assignment revoked successfully: {Id}", request.RoleDepartmentPolicyId);

            return new DeleteResponse
            {
                Success = true,
                Message = HumanResourceSuccessStrings.SUCCESS_REVOKE_PERMISSION
            };
        }
    }
}

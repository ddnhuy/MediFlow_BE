namespace HumanResource.Grpc.Database
{
    public static class SeedData
    {
        public static async Task InitializeDataAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext dbContext)
        {
            await EnsureRolesAsync(roleManager);
            await CreateAdminUserAsync(userManager);
            await CreateDepartmentsAsync(dbContext);
            await CreateDepartmentUsersAsync(userManager, dbContext);
            await CreateDepartmentPoliciesAsync(dbContext);
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new[]
            {
                Roles.ADMIN,
                Roles.DOCTOR,
                Roles.NURSE,
                Roles.PATIENT,
                Roles.LABORATORY_STAFF,
                Roles.PHARMACY_STAFF,
                Roles.WAREHOUSE_STAFF,
                Roles.ACCOUNTANT,
                Roles.RECEPTIONIST,
                Roles.IMAGING_TECHNICIAN,
                Roles.HEAD_OF_DEPARTMENT
            };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Create the initial administrator user
            string adminUserName = "mediflow";
            string adminPassword = "Mediflow@123";

            var adminUser = await userManager.FindByNameAsync(adminUserName);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = "admin@mediflow.health.vn",
                    Name = "MediFlow Admin",
                    Code = "MEDIFLOW000",
                    Gender = Gender.Male,
                    Address = "Khu đô thị FPT City, Ngũ Hành Sơn, Đà Nẵng, Việt Nam",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 0,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.ADMIN);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Seed admin user error: {error.Description}");
                    }
                }
            }
        }

        private static async Task CreateDepartmentsAsync(ApplicationDbContext dbContext)
        {
            if (!await dbContext.DepartmentTypes.AnyAsync())
            {
                var departmentTypes = new List<DepartmentType>
                {
                    new() { Code = nameof(DepartmentTypes.CLINIC), Name = DepartmentTypes.CLINIC },
                    new() { Code = nameof(DepartmentTypes.LAB), Name = DepartmentTypes.LAB },
                    new() { Code = nameof(DepartmentTypes.VACCINE), Name = DepartmentTypes.VACCINE },
                    new() { Code = nameof(DepartmentTypes.VACCINE_RECEPTION), Name = DepartmentTypes.VACCINE_RECEPTION },
                    new() { Code = nameof(DepartmentTypes.BILLING), Name = DepartmentTypes.BILLING },
                    new() { Code = nameof(DepartmentTypes.PHARMACY), Name = DepartmentTypes.PHARMACY },
                    new() { Code = nameof(DepartmentTypes.STORAGE), Name = DepartmentTypes.STORAGE },
                    new() { Code = nameof(DepartmentTypes.IMAGING), Name = DepartmentTypes.IMAGING },
                    new() { Code = nameof(DepartmentTypes.EMERGENCY), Name = DepartmentTypes.EMERGENCY },
                    new() { Code = nameof(DepartmentTypes.INPATIENT), Name = DepartmentTypes.INPATIENT },
                    new() { Code = nameof(DepartmentTypes.ADMIN), Name = DepartmentTypes.ADMIN },
                    new() { Code = nameof(DepartmentTypes.MANAGEMENT), Name = DepartmentTypes.MANAGEMENT },
                    new() { Code = nameof(DepartmentTypes.HR), Name = DepartmentTypes.HR },
                    new() { Code = nameof(DepartmentTypes.FINANCE), Name = DepartmentTypes.FINANCE }
                };

                await dbContext.DepartmentTypes.AddRangeAsync(departmentTypes);
                await dbContext.SaveChangesAsync();
            }

            var departmentTypeMap = await dbContext.DepartmentTypes
                .ToDictionaryAsync(dt => dt.Code, dt => dt.Id);

            if (!await dbContext.Departments.AnyAsync())
            {
                var adminUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == "mediflow");

                if (adminUser is not null)
                {
                    var departments = new List<Department>
                    {
                        new Department
                        {
                            Code = $"DEPT-000",
                            Name = $"Phòng khám tổng quát",
                            DepartmentTypeId = departmentTypeMap["CLINIC"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-001",
                            Name = $"Phòng xét nghiệm huyết học",
                            DepartmentTypeId = departmentTypeMap["LAB"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-1),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-002",
                            Name = $"Phòng tiêm chủng trẻ em",
                            DepartmentTypeId = departmentTypeMap["VACCINE"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-003",
                            Name = $"Phòng tiếp nhận tiêm chủng",
                            DepartmentTypeId = departmentTypeMap["VACCINE_RECEPTION"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-3),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-004",
                            Name = $"Phòng tài chính – thanh toán",
                            DepartmentTypeId = departmentTypeMap["BILLING"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-4),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-005",
                            Name = $"Phòng dược – cấp phát thuốc",
                            DepartmentTypeId = departmentTypeMap["PHARMACY"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-5),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-006",
                            Name = $"Kho vật tư y tế",
                            DepartmentTypeId = departmentTypeMap["STORAGE"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-6),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-007",
                            Name = $"Phòng chẩn đoán hình ảnh",
                            DepartmentTypeId = departmentTypeMap["IMAGING"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-7),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-008",
                            Name = $"Phòng cấp cứu",
                            DepartmentTypeId = departmentTypeMap["EMERGENCY"],
                            IsSuspended = true,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-8),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-009",
                            Name = $"Phòng nội trú",
                            DepartmentTypeId = departmentTypeMap["INPATIENT"],
                            IsSuspended = false,
                            IsCancelled = true,
                            CreatedAt = DateTime.UtcNow.AddDays(-9),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-010",
                            Name = $"Phòng quản trị hệ thống",
                            DepartmentTypeId = departmentTypeMap["ADMIN"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-10),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-011",
                            Name = $"Phòng ban giám đốc",
                            DepartmentTypeId = departmentTypeMap["MANAGEMENT"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-11),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-012",
                            Name = $"Phòng nhân sự",
                            DepartmentTypeId = departmentTypeMap["HR"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-12),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        },
                        new Department
                        {
                            Code = $"DEPT-013",
                            Name = $"Phòng kế toán – tài chính",
                            DepartmentTypeId = departmentTypeMap["FINANCE"],
                            IsSuspended = false,
                            IsCancelled = false,
                            CreatedAt = DateTime.UtcNow.AddDays(-13),
                            CreatedBy = adminUser.Id,
                            LastUpdatedAt = DateTime.UtcNow,
                            LastUpdatedBy = adminUser.Id
                        }
                    };

                    await dbContext.Departments.AddRangeAsync(departments);
                    await dbContext.SaveChangesAsync();

                    dbContext.ChangeTracker.Clear();

                    var department = await dbContext.Departments.FirstOrDefaultAsync(x => x.Name == "Phòng quản trị hệ thống");
                    if (department is not null)
                    {
                        adminUser.Departments = [department];
                        dbContext.Update(adminUser);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        private static async Task CreateDepartmentUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            string commonPassword = "Mediflow@123";

            // Department-role mapping
            var departmentRoleMap = new Dictionary<string, string>
            {
                ["CLINIC"] = Roles.DOCTOR,
                ["LAB"] = Roles.LABORATORY_STAFF,
                ["VACCINE"] = Roles.NURSE,
                ["VACCINE_RECEPTION"] = Roles.RECEPTIONIST,
                ["BILLING"] = Roles.ACCOUNTANT,
                ["PHARMACY"] = Roles.PHARMACY_STAFF,
                ["STORAGE"] = Roles.WAREHOUSE_STAFF,
                ["IMAGING"] = Roles.IMAGING_TECHNICIAN,
                ["EMERGENCY"] = Roles.DOCTOR,
                ["INPATIENT"] = Roles.NURSE,
                ["HEAD_OF_DEPARTMENT"] = Roles.HEAD_OF_DEPARTMENT
            };

            int userIndex = 1;
            foreach (var kvp in departmentRoleMap)
            {
                var userName = $"user{userIndex:D3}";
                var email = $"user{userIndex:D3}@mediflow.health.vn";

                if (await userManager.FindByNameAsync(userName) is not null)
                {
                    userIndex++;
                    continue;
                }

                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    Name = $"User {userIndex}",
                    Code = $"USER{userIndex:D3}",
                    Gender = userIndex % 2 == 0 ? Gender.Male : Gender.Female,
                    Address = "Khu đô thị FPT City, Ngũ Hành Sơn, Đà Nẵng",
                    CreatedAt = DateTime.UtcNow.AddDays(-userIndex),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(user, commonPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, kvp.Value);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Seed department user error: {error.Description}");
                    }
                }

                userIndex++;
            }

            // Seed Head of Department user
            var headUserName = "ceo01";
            var headEmail = "ceo01@mediflow.health.vn";

            if (await userManager.FindByNameAsync(headUserName) is null)
            {
                var headUser = new ApplicationUser
                {
                    UserName = headUserName,
                    Email = headEmail,
                    Name = "Head of Department",
                    Code = "HEAD001",
                    Gender = Gender.Male,
                    Address = "Khu đô thị FPT City, Ngũ Hành Sơn, Đà Nẵng",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(headUser, commonPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(headUser, Roles.HEAD_OF_DEPARTMENT);

                    // Assign departments to department heads
                    var department = await dbContext.Departments.FirstOrDefaultAsync(d => d.Name == "Phòng ban giám đốc");

                    if (department is not null)
                    {
                        headUser.Departments = [department];
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Seed head of department user error: {error.Description}");
                    }
                }
            }
        }

        private static async Task CreateDepartmentPoliciesAsync(ApplicationDbContext dbContext)
        {
            var departments = await dbContext.Departments
                .Include(d => d.DepartmentType)
                .Where(d => !d.IsCancelled)
                .ToListAsync();

            var roles = await dbContext.Roles.ToListAsync();

            // Ensure all policies exist
            foreach (var group in PredefinedPolicies)
            {
                foreach (var policy in group.Value)
                {
                    if (!await dbContext.Policies.AnyAsync(p => p.ResourceType == policy.ResourceType))
                    {
                        await dbContext.Policies.AddAsync(policy);
                    }
                }
            }

            await dbContext.SaveChangesAsync();

            var allPolicies = await dbContext.Policies.ToListAsync();

            foreach (var (resourceType, roleDeptList) in RoleDepartmentMappings)
            {
                var relatedPolicies = allPolicies.Where(p => p.ResourceType == resourceType).ToList();
                if (!relatedPolicies.Any()) continue;

                foreach (var (roleName, deptTypeName) in roleDeptList)
                {
                    var role = roles.FirstOrDefault(r => r.Name == roleName);
                    var department = departments.FirstOrDefault(d => d.DepartmentType.Name == deptTypeName);

                    if (role == null || department == null) continue;

                    foreach (var policy in relatedPolicies)
                    {
                        bool alreadyMapped = await dbContext.RoleDepartmentPolicies.AnyAsync(rdp =>
                            rdp.RoleId == role.Id &&
                            rdp.DepartmentId == department.Id &&
                            rdp.PolicyId == policy.Id);

                        if (!alreadyMapped)
                        {
                            await dbContext.RoleDepartmentPolicies.AddAsync(new RoleDepartmentPolicy
                            {
                                RoleId = role.Id,
                                DepartmentId = department.Id,
                                PolicyId = policy.Id
                            });
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private static readonly Dictionary<string, List<Policy>> PredefinedPolicies = new()
        {
            ["inventory"] = new()
            {
                new Policy { ResourceType = "inventory", Actions = ["read", "write"] }
            },
            ["user"] = new()
            {
                new Policy { ResourceType = "user", Actions = ["read", "write"] }
            }
        };

        private static readonly Dictionary<string, List<(string roleName, string departmentType)>> RoleDepartmentMappings = new()
        {
            ["inventory"] = new()
            {
                (Roles.WAREHOUSE_STAFF, DepartmentTypes.STORAGE),
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT)
            },
            ["user"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT)
            }
        };
    }
}
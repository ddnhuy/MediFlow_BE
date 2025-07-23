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
                Roles.HEAD_OF_DEPARTMENT,
                Roles.IT_SUPPORT
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
                    new() { Code = nameof(DepartmentTypes.CLINIC), Name = DepartmentTypes.CLINIC, NameInEnglish = nameof(DepartmentTypes.CLINIC).ToLower() },
                    new() { Code = nameof(DepartmentTypes.LAB), Name = DepartmentTypes.LAB, NameInEnglish = nameof(DepartmentTypes.LAB).ToLower() },
                    new() { Code = nameof(DepartmentTypes.VACCINE), Name = DepartmentTypes.VACCINE, NameInEnglish = nameof(DepartmentTypes.VACCINE).ToLower() },
                    new() { Code = nameof(DepartmentTypes.VACCINE_RECEPTION), Name = DepartmentTypes.VACCINE_RECEPTION, NameInEnglish = nameof(DepartmentTypes.VACCINE_RECEPTION).ToLower() },
                    new() { Code = nameof(DepartmentTypes.BILLING), Name = DepartmentTypes.BILLING, NameInEnglish = nameof(DepartmentTypes.BILLING).ToLower() },
                    new() { Code = nameof(DepartmentTypes.PHARMACY), Name = DepartmentTypes.PHARMACY, NameInEnglish = nameof(DepartmentTypes.PHARMACY).ToLower() },
                    new() { Code = nameof(DepartmentTypes.STORAGE), Name = DepartmentTypes.STORAGE, NameInEnglish = nameof(DepartmentTypes.STORAGE).ToLower() },
                    new() { Code = nameof(DepartmentTypes.IMAGING), Name = DepartmentTypes.IMAGING, NameInEnglish = nameof(DepartmentTypes.IMAGING).ToLower() },
                    new() { Code = nameof(DepartmentTypes.EMERGENCY), Name = DepartmentTypes.EMERGENCY, NameInEnglish = nameof(DepartmentTypes.EMERGENCY).ToLower() },
                    new() { Code = nameof(DepartmentTypes.INPATIENT), Name = DepartmentTypes.INPATIENT, NameInEnglish = nameof(DepartmentTypes.INPATIENT).ToLower() },
                    new() { Code = nameof(DepartmentTypes.ADMIN), Name = DepartmentTypes.ADMIN, NameInEnglish = nameof(DepartmentTypes.ADMIN).ToLower() },
                    new() { Code = nameof(DepartmentTypes.MANAGEMENT), Name = DepartmentTypes.MANAGEMENT, NameInEnglish = nameof(DepartmentTypes.MANAGEMENT).ToLower() },
                    new() { Code = nameof(DepartmentTypes.HR), Name = DepartmentTypes.HR, NameInEnglish = nameof(DepartmentTypes.HR).ToLower() },
                    new() { Code = nameof(DepartmentTypes.FINANCE), Name = DepartmentTypes.FINANCE, NameInEnglish = nameof(DepartmentTypes.FINANCE).ToLower() }
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
                            NameInEnglish = "General Clinic",
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
                            NameInEnglish = "Hematology Lab",
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
                            NameInEnglish = "Child Vaccination Room",
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
                            NameInEnglish = "Vaccination Reception",
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
                            NameInEnglish = "Billing Department",
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
                            NameInEnglish = "Pharmacy Department",
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
                            NameInEnglish = "Medical Supplies Storage",
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
                            NameInEnglish = "Imaging Department",
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
                            NameInEnglish = "Emergency Department",
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
                            NameInEnglish = "Inpatient Department",
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
                            NameInEnglish = "System Administration",
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
                            NameInEnglish = "Management Department",
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
                            NameInEnglish = "Human Resources",
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
                            NameInEnglish = "Finance and Accounting",
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
            int userIndex = 1;

            // Seed users according to Role - DepartmentType mapping to ensure policy coverage
            foreach (var (resourceType, roleDeptList) in RoleDepartmentMappings)
            {
                foreach (var (roleName, departmentTypeName) in roleDeptList)
                {
                    var department = await dbContext.Departments
                        .Include(d => d.DepartmentType)
                        .FirstOrDefaultAsync(d => d.DepartmentType.Name == departmentTypeName);

                    if (department == null)
                        continue;

                    // Make sure not to create duplicate users
                    string userName = $"user{userIndex:D3}";
                    string email = $"{userName}@mediflow.health.vn";

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
                        PhoneNumberConfirmed = true,
                        Departments = new List<Department> { department }
                    };

                    var result = await userManager.CreateAsync(user, commonPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Seed user error ({userName}): {error.Description}");
                        }
                    }

                    userIndex++;
                }
            }

            // Seed a special user for Head of Department (if not already present)
            var headUserName = "ceo01";
            if (await userManager.FindByNameAsync(headUserName) is null)
            {
                var headDepartment = await dbContext.Departments
                    .Include(d => d.DepartmentType)
                    .FirstOrDefaultAsync(d => d.DepartmentType.Name == DepartmentTypes.MANAGEMENT);

                if (headDepartment != null)
                {
                    var headUser = new ApplicationUser
                    {
                        UserName = headUserName,
                        Email = "ceo01@mediflow.health.vn",
                        Name = "Head of Department",
                        Code = "HEAD001",
                        Gender = Gender.Male,
                        Address = "Khu đô thị FPT City, Ngũ Hành Sơn, Đà Nẵng",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        Departments = new List<Department> { headDepartment }
                    };

                    var result = await userManager.CreateAsync(headUser, commonPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(headUser, Roles.HEAD_OF_DEPARTMENT);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Seed ceo01 error: {error.Description}");
                        }
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
            ["management"] = new()
            {
                new Policy { ResourceType = "management", Actions = ["read", "write"] }
            },
            ["vaccination-reception"] = new()
            {
                new Policy { ResourceType = "vaccination-reception", Actions = ["read", "write"] }
            },
            ["file-storage"] = new()
            {
                new Policy { ResourceType = "file-storage", Actions = ["read", "write"] }
            },
            ["appointments"] = new()
            {
                new Policy { ResourceType = "appointments", Actions = ["read", "write"] }
            },
            ["hospital-service"] = new()
            {
                new Policy { ResourceType = "hospital-service", Actions = ["read", "write"] }
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
            ["management"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT)
            },
            ["vaccination-reception"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.RECEPTIONIST, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.LABORATORY_STAFF, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.DOCTOR, DepartmentTypes.CLINIC),
                (Roles.DOCTOR, DepartmentTypes.VACCINE),
                (Roles.DOCTOR, DepartmentTypes.LAB),
                (Roles.DOCTOR, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.NURSE, DepartmentTypes.CLINIC),
                (Roles.NURSE, DepartmentTypes.VACCINE),
                (Roles.NURSE, DepartmentTypes.LAB),
                (Roles.NURSE, DepartmentTypes.VACCINE_RECEPTION),
            },
            ["file-storage"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.RECEPTIONIST, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.DOCTOR, DepartmentTypes.CLINIC),
                (Roles.DOCTOR, DepartmentTypes.VACCINE),
                (Roles.DOCTOR, DepartmentTypes.LAB),
                (Roles.NURSE, DepartmentTypes.CLINIC),
                (Roles.NURSE, DepartmentTypes.VACCINE),
                (Roles.NURSE, DepartmentTypes.LAB),
                (Roles.IMAGING_TECHNICIAN, DepartmentTypes.IMAGING),
                (Roles.LABORATORY_STAFF, DepartmentTypes.LAB),
                (Roles.PHARMACY_STAFF, DepartmentTypes.PHARMACY),
                (Roles.WAREHOUSE_STAFF, DepartmentTypes.STORAGE),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT),
                (Roles.ACCOUNTANT, DepartmentTypes.FINANCE),
                (Roles.IT_SUPPORT, DepartmentTypes.ADMIN)
            },
            ["appointments"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.RECEPTIONIST, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.DOCTOR, DepartmentTypes.CLINIC),
                (Roles.DOCTOR, DepartmentTypes.VACCINE),
                (Roles.DOCTOR, DepartmentTypes.LAB),
                (Roles.NURSE, DepartmentTypes.CLINIC),
                (Roles.NURSE, DepartmentTypes.VACCINE),
                (Roles.NURSE, DepartmentTypes.LAB),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT)
            },
            ["hospital-service"] = new()
            {
                (Roles.ADMIN, DepartmentTypes.ADMIN),
                (Roles.RECEPTIONIST, DepartmentTypes.VACCINE_RECEPTION),
                (Roles.DOCTOR, DepartmentTypes.CLINIC),
                (Roles.DOCTOR, DepartmentTypes.VACCINE),
                (Roles.DOCTOR, DepartmentTypes.LAB),
                (Roles.NURSE, DepartmentTypes.CLINIC),
                (Roles.NURSE, DepartmentTypes.VACCINE),
                (Roles.NURSE, DepartmentTypes.LAB),
                (Roles.HEAD_OF_DEPARTMENT, DepartmentTypes.MANAGEMENT)
            }
        };
    }
}
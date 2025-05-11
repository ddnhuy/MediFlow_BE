using BuildingBlocks.Strings;
using HumanResource.Grpc.Database;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static async Task InitializeUserData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new[] { Roles.ADMIN, Roles.DOCTOR, Roles.NURSE };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            string userName = "mediflow";
            string password = "Mediflow@123";

            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = "admin@mediflow.com",
                    Name = "MediFlow Admin",
                    Code = "MEDIFLOW000",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 0,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.ADMIN);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Seed user error: {error.Description}");
                    }
                }
            }
        }

        public static async Task InitializeDepartmentData(ApplicationDbContext dbContext)
        {
            if (!await dbContext.DepartmentTypes.AnyAsync())
            {
                var departmentTypes = new List<DepartmentType>
            {
                new DepartmentType { Code = "CLINIC", Name = "Phòng khám" },
                new DepartmentType { Code = "LAB", Name = "Phòng xét nghiệm" },
                new DepartmentType { Code = "VACCINE", Name = "Phòng tiêm chủng" }
            };

                await dbContext.DepartmentTypes.AddRangeAsync(departmentTypes);
                await dbContext.SaveChangesAsync();
            }

            var typeIds = await dbContext.DepartmentTypes.Select(dt => dt.Id).ToListAsync();

            if (!await dbContext.Departments.AnyAsync())
            {
                var departments = new List<Department>();

                for (int i = 1; i <= 10; i++)
                {
                    departments.Add(new Department
                    {
                        Code = $"DEPT-{i:000}",
                        Name = $"Department {i}",
                        DepartmentTypeId = typeIds[(i - 1) % typeIds.Count],
                        IsSuspended = i % 4 == 0,
                        IsCancelled = i % 5 == 0,
                        CreatedAt = DateTime.UtcNow.AddDays(-i),
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    });
                }

                await dbContext.Departments.AddRangeAsync(departments);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
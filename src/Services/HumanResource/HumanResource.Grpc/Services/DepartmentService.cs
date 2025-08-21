using Google.Protobuf.WellKnownTypes;

namespace HumanResource.Grpc.Services
{
    public class DepartmentService(
        ApplicationDbContext dbContext,
        ILogger<DepartmentService> logger)
        : DepartmentProtoService.DepartmentProtoServiceBase
    {
        public override async Task<ListDepartmentsResponse> ListDepartments(ListDepartmentsRequest request, ServerCallContext context)
        {
            logger.LogInformation("Listing departments. Keyword: {Keyword}, PageIndex: {PageIndex}, PageSize: {PageSize}", request.Keyword, request.PageIndex, request.PageSize);

            var result = new ListDepartmentsResponse();

            var query = dbContext.Departments.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string keyword = request.Keyword.Trim().ToLower();
                query = query.Where(
                    x => x.Code.ToLower().Contains(keyword)
                    || x.Name.ToLower().Contains(keyword)
                    || x.NameInEnglish.ToLower().Contains(keyword));
            }

            result.Count = await query.CountAsync();

            var departmentList = await query
                .Where(x => !x.IsCancelled)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(x => x.DepartmentType)
                .ToListAsync();

            logger.LogInformation("Found {Count} departments matching the criteria.", result.Count);

            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            result.Data.AddRange(departmentList.Select(department => new DepartmentSummaryModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                NameInEnglish = department.NameInEnglish,
                DepartmentTypeName = department.DepartmentType.Name,
                DepartmentTypeNameInEnglish = department.DepartmentType.NameInEnglish,
                IsSuspended = department.IsSuspended
            }));

            return result;
        }

        public override async Task<DepartmentDetailModel> GetDepartment(GetDepartmentRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting department with id={Id}", request.Id);

            var department = await dbContext.Departments
                .Include(d => d.DepartmentType)
                .Where(x => !x.IsCancelled)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (department is null)
            {
                logger.LogWarning("Department with id={Id} not found.", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_DEPARTMENT_WITH_ID.ToString()));
            }

            logger.LogInformation("Department with id={Id} retrieved successfully.", request.Id);

            var departmentModel = new DepartmentDetailModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                NameInEnglish = department.NameInEnglish,
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
                DepartmentTypeNameInEnglish = department.DepartmentType.NameInEnglish,
                IsSuspended = department.IsSuspended,
                IsCancelled = department.IsCancelled,
                CreatedAt = Timestamp.FromDateTime(department.CreatedAt),
                CreatedBy = department.CreatedBy,
                LastUpdatedAt = Timestamp.FromDateTime(department.LastUpdatedAt),
                LastUpdatedBy = department.LastUpdatedBy
            };

            return departmentModel;
        }

        public override async Task<DepartmentDetailModel> CreateDepartment(CreateDepartmentRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new department with name={Name}", request.Name);

            var department = request.Adapt<Department>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, ExceptionKey.INVALID_REQUEST.ToString()));

            var checkIfDepartmentCodeExist = await dbContext.Departments.AnyAsync(x => x.Code == request.Code && !x.IsCancelled);
            if (checkIfDepartmentCodeExist)
            {
                logger.LogWarning("Department code {Code} already exists.", request.Code);
                throw new RpcException(new Status(StatusCode.AlreadyExists, ExceptionKey.EXISTED_DEPARTMENT_CODE.ToString()));
            }

            var departmentType = await dbContext.DepartmentTypes.FirstOrDefaultAsync(x => x.Id == request.DepartmentTypeId);
            if (departmentType is null)
            {
                logger.LogWarning("Invalid department type id={Id} during creation.", request.DepartmentTypeId);
                throw new RpcException(new Status(StatusCode.InvalidArgument, ExceptionKey.INVALID_DEPARTMENT_TYPE.ToString()));
            }

            department.DepartmentType = departmentType;

            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Department successfully created. Name: {DepartmentName}, ID: {DepartmentId}", department.Name, department.Id);

            var departmentModel = new DepartmentDetailModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                NameInEnglish = department.NameInEnglish,
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
                DepartmentTypeNameInEnglish = department.DepartmentType.NameInEnglish,
                IsSuspended = department.IsSuspended,
                IsCancelled = department.IsCancelled,
                CreatedAt = Timestamp.FromDateTime(department.CreatedAt),
                CreatedBy = department.CreatedBy,
                LastUpdatedAt = Timestamp.FromDateTime(department.LastUpdatedAt),
                LastUpdatedBy = department.LastUpdatedBy
            };

            return departmentModel;
        }

        public override async Task<DepartmentDetailModel> UpdateDepartment(UpdateDepartmentRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating department with id={Id}", request.Id);

            var department = await dbContext.Departments
                .Include(d => d.DepartmentType)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (department == null)
            {
                logger.LogWarning("Department with id={Id} not found for update.", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_DEPARTMENT_WITH_ID.ToString()));
            }

            var checkIfDepartmentCodeExist = await dbContext.Departments.AnyAsync(x => x.Id != request.Id && (x.Code == request.Code && !x.IsCancelled));
            if (checkIfDepartmentCodeExist)
            {
                logger.LogWarning("Department code {Code} already exists.", request.Code);
                throw new RpcException(new Status(StatusCode.AlreadyExists, ExceptionKey.EXISTED_DEPARTMENT_CODE.ToString()));
            }

            var departmentType = await dbContext.DepartmentTypes.FirstOrDefaultAsync(x => x.Id == request.DepartmentTypeId);
            if (departmentType is null)
            {
                logger.LogWarning("Invalid department type id={Id} during update.", request.DepartmentTypeId);
                throw new RpcException(new Status(StatusCode.InvalidArgument, ExceptionKey.INVALID_DEPARTMENT_TYPE.ToString()));
            }

            department.DepartmentType = departmentType;
            department.Code = request.Code;
            department.Name = request.Name;
            department.NameInEnglish = request.NameInEnglish;
            department.IsSuspended = request.IsSuspended;
            department.IsCancelled = request.IsCancelled;

            dbContext.Departments.Update(department);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Department successfully updated. ID: {Id}, New Name: {Name}", department.Id, department.Name);

            var departmentModel = new DepartmentDetailModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                NameInEnglish = department.NameInEnglish,
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
                DepartmentTypeNameInEnglish = department.DepartmentType.NameInEnglish,
                IsSuspended = department.IsSuspended,
                IsCancelled = department.IsCancelled,
                CreatedAt = Timestamp.FromDateTime(department.CreatedAt),
                CreatedBy = department.CreatedBy,
                LastUpdatedAt = Timestamp.FromDateTime(department.LastUpdatedAt),
                LastUpdatedBy = department.LastUpdatedBy
            };

            return departmentModel;
        }

        public override async Task<DeleteDepartmentResponse> DeleteDepartment(DeleteDepartmentRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting department with id={Id}", request.Id);

            var department = await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsCancelled);

            if (department is null)
            {
                logger.LogWarning("Department with id={Id} not found for deletion.", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_DEPARTMENT_WITH_ID.ToString()));
            }

            department.IsSuspended = true;
            department.IsCancelled = true;

            dbContext.Departments.Update(department);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Department with id={Id} deleted successfully.", request.Id);

            return new DeleteDepartmentResponse { IsSuccess = true };
        }

        public override async Task<ListEmployeesResponse> ListEmployees(ListEmployeesRequest request, ServerCallContext context)
        {
            logger.LogInformation("Listing employees for department with id={Id}", request.Id);

            var result = new ListEmployeesResponse();

            var department = await dbContext.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsCancelled);

            if (department is null)
            {
                logger.LogWarning("Department with id={Id} not found for employee listing.", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ExceptionKey.NOT_FOUND_DEPARTMENT_WITH_ID.ToString()));
            }

            logger.LogInformation("Found {Count} employees in department with id={Id}.", department.Users.Count(), request.Id);

            var employeeList = department.Users
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(user => new EmployeeSummaryModel
                {
                    Id = user.Id,
                    Code = user.Code,
                    Name = user.Name,
                    IsSuspended = user.IsSuspended,
                    ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty
                })
                .ToList();

            result.Count = department.Users.Count();
            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            result.Data.AddRange(employeeList);

            return result;
        }
    }
}
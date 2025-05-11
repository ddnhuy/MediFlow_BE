using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HumanResource.Grpc.Database;

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
                query = query.Where(x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword));
            }

            result.Count = await query.CountAsync();

            var departmentList = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(x => x.DepartmentType)
                .Where(x => !x.IsCancelled)
                .ToListAsync();

            logger.LogInformation("Found {Count} departments matching the criteria.", result.Count);

            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            result.Data.AddRange(departmentList.Select(department => new DepartmentSummaryModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                DepartmentTypeName = department.DepartmentType.Name,
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
                throw new RpcException(new Status(StatusCode.NotFound, $"Department with id={request.Id} is not found."));
            }

            logger.LogInformation("Department with id={Id} retrieved successfully.", request.Id);

            var departmentModel = new DepartmentDetailModel
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
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

            var department = request.Adapt<Department>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

            var departmentType = await dbContext.DepartmentTypes.FirstOrDefaultAsync(x => x.Id == request.DepartmentTypeId);
            if (departmentType is null)
            {
                logger.LogWarning("Invalid department type id={Id} during creation.", request.DepartmentTypeId);
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid department type."));
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
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
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
                throw new RpcException(new Status(StatusCode.NotFound, $"Department with id={request.Id} is not found."));
            }

            var departmentType = await dbContext.DepartmentTypes.FirstOrDefaultAsync(x => x.Id == request.DepartmentTypeId);
            if (departmentType is null)
            {
                logger.LogWarning("Invalid department type id={Id} during update.", request.DepartmentTypeId);
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid department type."));
            }

            department.DepartmentType = departmentType;
            department.Code = request.Code;
            department.Name = request.Name;
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
                DepartmentTypeId = department.DepartmentType.Id,
                DepartmentTypeName = department.DepartmentType.Name,
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

            var department = await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (department is null)
            {
                logger.LogWarning("Department with id={Id} not found for deletion.", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, $"Department with id={request.Id} is not found."));
            }

            department.IsSuspended = true;
            department.IsCancelled = true;

            dbContext.Departments.Update(department);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Department with id={Id} deleted successfully.", request.Id);

            return new DeleteDepartmentResponse { IsSuccess = true };
        }
    }
}
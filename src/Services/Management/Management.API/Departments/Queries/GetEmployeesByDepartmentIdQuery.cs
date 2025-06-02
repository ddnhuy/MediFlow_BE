using Management.API.Dtos.User;

namespace Management.API.Departments.Queries
{
    public record GetEmployeesByDepartmentIdResult(PaginatedResult<EmployeeSummaryDto> EmployeeList);
    public record GetEmployeesByDepartmentIdQuery(int DepartmentId, int PageIndex, int PageSize) : IQuery<GetEmployeesByDepartmentIdResult>;

    internal class GetEmployeesByDepartmentIdQueryHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto) : IQueryHandler<GetEmployeesByDepartmentIdQuery, GetEmployeesByDepartmentIdResult>
    {
        public async Task<GetEmployeesByDepartmentIdResult> Handle(GetEmployeesByDepartmentIdQuery query, CancellationToken cancellationToken)
        {
            var result = await departmentProto.ListEmployeesAsync(new ListEmployeesRequest
            {
                Id = query.DepartmentId,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            }, cancellationToken: cancellationToken);

            return new GetEmployeesByDepartmentIdResult(new PaginatedResult<EmployeeSummaryDto>(
                result.PageIndex,
                result.PageSize,
                result.Count,
                result.Data.Adapt<IEnumerable<EmployeeSummaryDto>>()));
        }
    }
}

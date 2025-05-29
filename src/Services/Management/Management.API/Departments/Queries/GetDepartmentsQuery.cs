using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;

namespace Management.API.Departments.Queries
{
    public record GetDepartmentsResult(PaginatedResult<DepartmentSummaryDto> Departments);
    public record GetDepartmentsQuery(int PageIndex, int PageSize, string? Keyword) : IQuery<GetDepartmentsResult>;

    internal class GetDepartmentsQueryHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto) : IQueryHandler<GetDepartmentsQuery, GetDepartmentsResult>
    {
        public async Task<GetDepartmentsResult> Handle(GetDepartmentsQuery query, CancellationToken cancellationToken)
        {
            var result = await departmentProto.ListDepartmentsAsync(new ListDepartmentsRequest
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Keyword = query.Keyword ?? string.Empty
            }, cancellationToken: cancellationToken);

            var data = result.Data.Select(department => new DepartmentSummaryDto
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                NameInEnglish = department.NameInEnglish,
                DepartmentType = new DepartmentTypeSummaryDto
                {
                    Name = department.DepartmentTypeName,
                    NameInEnglish = department.DepartmentTypeNameInEnglish
                },
                IsSuspended = department.IsSuspended
            });

            return new GetDepartmentsResult(
                new PaginatedResult<DepartmentSummaryDto>(
                    result.PageIndex,
                    result.PageSize,
                    result.Count,
                    data));
        }
    }
}

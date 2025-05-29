using Management.API.Helpers;

namespace Management.API.Departments.Queries
{
    public record GetDepartmentsResult(PaginatedResult<DepartmentSummaryModel> Departments);
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

            return new GetDepartmentsResult(
                new PaginatedResult<DepartmentSummaryModel>(
                    result.PageIndex,
                    result.PageSize,
                    result.Count,
                    result.Data));
        }
    }
}

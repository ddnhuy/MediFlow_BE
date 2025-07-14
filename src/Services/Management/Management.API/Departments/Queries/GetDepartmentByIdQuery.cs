using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;

namespace Management.API.Departments.Queries
{
    public record GetDepartmentByIdResult(DepartmentDetailDto Department);
    public record GetDepartmentByIdQuery(int DepartmentId) : IQuery<GetDepartmentByIdResult>;

    internal class GetDepartmentByIdQueryHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto) : IQueryHandler<GetDepartmentByIdQuery, GetDepartmentByIdResult>
    {
        public async Task<GetDepartmentByIdResult> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await departmentProto.GetDepartmentAsync(new GetDepartmentRequest
            {
                Id = query.DepartmentId
            }, cancellationToken: cancellationToken);

            return new GetDepartmentByIdResult(new DepartmentDetailDto
            {
                Id = result.Id,
                Name = result.Name,
                NameInEnglish = result.NameInEnglish,
                Code = result.Code,
                DepartmentType = new DepartmentTypeDetailDto
                {
                    Id = result.DepartmentTypeId,
                    Name = result.DepartmentTypeName,
                    NameInEnglish = result.DepartmentTypeNameInEnglish
                },
                IsSuspended = result.IsSuspended,
                IsCancelled = result.IsCancelled,
                CreatedAt = result.CreatedAt.ToDateTime(),
                LastUpdatedAt = result.LastUpdatedAt.ToDateTime()
            });
        }
    }
}

using Management.API.Dtos.DepartmentType;

namespace Management.API.DepartmentTypes.Queries
{
    public record GetDepartmentTypesResult(IEnumerable<DepartmentTypeDetailDto> DepartmentTypes);
    public record GetDepartmentTypesQuery() : IQuery<GetDepartmentTypesResult>;

    internal class GetDepartmentTypesQueryHandler(
        DepartmentTypeProtoService.DepartmentTypeProtoServiceClient departmentProto) : IQueryHandler<GetDepartmentTypesQuery, GetDepartmentTypesResult>
    {
        public async Task<GetDepartmentTypesResult> Handle(GetDepartmentTypesQuery query, CancellationToken cancellationToken)
        {
            var result = await departmentProto.ListDepartmentTypesAsync(new ListDepartmentTypesRequest(), cancellationToken: cancellationToken);

            if (result.Data.Count == 0)
            {
                return new GetDepartmentTypesResult(Enumerable.Empty<DepartmentTypeDetailDto>());
            }

            return new GetDepartmentTypesResult(
                result.Data.Select(x => new DepartmentTypeDetailDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    NameInEnglish = x.NameInEnglish
                }));
        }
    }
}

namespace HumanResource.Grpc.Services
{
    public class DepartmentTypeService(
        ApplicationDbContext dbContext)
        : DepartmentTypeProtoService.DepartmentTypeProtoServiceBase
    {
        public override async Task<ListDepartmentTypesResponse> ListDepartmentTypes(ListDepartmentTypesRequest request, ServerCallContext context)
        {
            var departmentTypeList = await dbContext.DepartmentTypes.ToListAsync(context.CancellationToken);

            return new ListDepartmentTypesResponse
            {
                Data = { departmentTypeList.Select(x => new DepartmentTypeModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameInEnglish = x.NameInEnglish
                }) }
            };
        }
    }
}

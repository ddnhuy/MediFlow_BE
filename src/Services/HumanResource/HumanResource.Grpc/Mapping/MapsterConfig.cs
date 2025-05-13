namespace HumanResource.Grpc.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // DepartmentType -> DepartmentTypeModel
            config.NewConfig<DepartmentType, DepartmentTypeModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code ?? "")
                .Map(dest => dest.Name, src => src.Name ?? "")
                .TwoWays();

            // Department -> DepartmentDetailModel
            config.NewConfig<Department, DepartmentDetailModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code ?? "")
                .Map(dest => dest.Name, src => src.Name ?? "")
                .Map(dest => dest.DepartmentTypeId, src => src.DepartmentTypeId)
                .Map(dest => dest.DepartmentTypeName, src => src.DepartmentType.Name)
                .Map(dest => dest.IsSuspended, src => src.IsSuspended)
                .Map(dest => dest.IsCancelled, src => src.IsCancelled)
                .Map(dest => dest.CreatedAt, src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime()))
                .Map(dest => dest.CreatedBy, src => src.CreatedBy)
                .Map(dest => dest.LastUpdatedAt, src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.LastUpdatedAt.ToUniversalTime()))
                .Map(dest => dest.LastUpdatedBy, src => src.LastUpdatedBy)
                .TwoWays();

            // Department -> DepartmentSummaryModel
            config.NewConfig<Department, DepartmentSummaryModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code ?? "")
                .Map(dest => dest.Name, src => src.Name ?? "")
                .Map(dest => dest.DepartmentTypeName, src => src.DepartmentType.Name)
                .Map(dest => dest.IsSuspended, src => src.IsSuspended)
                .TwoWays();

            // ApplicationUser -> ApplicationUserDetailModel
            config.NewConfig<ApplicationUser, ApplicationUserDetailModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserName, src => src.UserName ?? "")
                .Map(dest => dest.Email, src => src.Email ?? "")
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber ?? "")
                .Map(dest => dest.Code, src => src.Code ?? "")
                .Map(dest => dest.Name, src => src.Name ?? "")
                .Map(dest => dest.EmailConfirmed, src => src.EmailConfirmed)
                .Map(dest => dest.PhoneNumberConfirmed, src => src.PhoneNumberConfirmed)
                .Map(dest => dest.TwoFactorEnabled, src => src.TwoFactorEnabled)
                .Map(dest => dest.IsSuspended, src => src.IsSuspended)
                .Map(dest => dest.IsCancelled, src => src.IsCancelled)
                .Map(dest => dest.CreatedAt, src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime()))
                .Map(dest => dest.CreatedBy, src => src.CreatedBy)
                .Map(dest => dest.LastUpdatedAt, src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.LastUpdatedAt.ToUniversalTime()))
                .Map(dest => dest.LastUpdatedBy, src => src.LastUpdatedBy)
                .Map(dest => dest.Departments, src => src.Departments.Adapt<List<DepartmentSummaryModel>>())
                .TwoWays();

            // ApplicationUser -> ApplicationUserSummaryModel
            config.NewConfig<ApplicationUser, ApplicationUserSummaryModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserName, src => src.UserName ?? "")
                .Map(dest => dest.Email, src => src.Email ?? "")
                .Map(dest => dest.Code, src => src.Code ?? "")
                .Map(dest => dest.Name, src => src.Name ?? "")
                .Map(dest => dest.IsSuspended, src => src.IsSuspended)
                .TwoWays();
        }
    }
}
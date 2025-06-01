using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Consts;

namespace VaccinationReception.Infrastructure.Data.SeedDatas
{
    public static class ServiceSeedData
    {
        private static Service SetBaseProperties(Service entity)
        {
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            return entity;
        }

        public static IEnumerable<Service> GetSeedData()
        {
            return new List<Service>
            {
                SetBaseProperties(new Service
                {
                    Id = 1,
                    ServiceCode = SeedConstants.Services.Vac5in1Code,
                    ServiceName = SeedConstants.Services.Vac5in1,
                    UnitPrice = SeedConstants.Services.Vac5in1Price,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                }),
                SetBaseProperties(new Service
                {
                    Id = 2,
                    ServiceCode = SeedConstants.Services.Vac6in1Code,
                    ServiceName = SeedConstants.Services.Vac6in1,
                    UnitPrice = SeedConstants.Services.Vac6in1Price,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                })
            };
        }
    }
}
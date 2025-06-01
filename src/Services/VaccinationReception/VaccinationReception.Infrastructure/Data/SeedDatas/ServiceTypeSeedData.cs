using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Consts;

namespace VaccinationReception.Infrastructure.Data.SeedDatas
{
    public static class ServiceTypeSeedData
    {
        private static ServiceType SetBaseProperties(ServiceType entity)
        {
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            return entity;
        }

        public static IEnumerable<ServiceType> GetSeedData()
        {
            return new List<ServiceType>
            {
                SetBaseProperties(new ServiceType
                {
                    Id = 1,
                    Code = SeedConstants.ServiceTypes.BasicCode,
                    Name = SeedConstants.ServiceTypes.BasicName
                }),
                SetBaseProperties(new ServiceType
                {
                    Id = 2,
                    Code = SeedConstants.ServiceTypes.SpecialCode,
                    Name = SeedConstants.ServiceTypes.SpecialName
                })
            };
        }
    }
}
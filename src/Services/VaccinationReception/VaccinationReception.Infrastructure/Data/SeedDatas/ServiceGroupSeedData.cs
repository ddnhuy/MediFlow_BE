using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Consts;

namespace VaccinationReception.Infrastructure.Data.SeedDatas
{
    public static class ServiceGroupSeedData
    {
        private static ServiceGroup SetBaseProperties(ServiceGroup entity)
        {
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            return entity;
        }

        public static IEnumerable<ServiceGroup> GetSeedData()
        {
            return new List<ServiceGroup>
            {
                SetBaseProperties(new ServiceGroup
                {
                    Id = 1,
                    GroupName = SeedConstants.ServiceGroups.BasicVaccination
                }),
                SetBaseProperties(new ServiceGroup
                {
                    Id = 2,
                    GroupName = SeedConstants.ServiceGroups.SpecialVaccination
                })
            };
        }
    }
}
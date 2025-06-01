using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Consts;

namespace VaccinationReception.Infrastructure.Data.SeedDatas
{
    public static class DiseaseGroupServiceSeedData
    {
        private static DiseaseGroupService SetBaseProperties(DiseaseGroupService entity)
        {
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            return entity;
        }

        public static IEnumerable<DiseaseGroupService> GetSeedData()
        {
            return new List<DiseaseGroupService>
            {
                SetBaseProperties(new DiseaseGroupService
                {
                    Id = 1,
                    DiseaseGroupId = 1,
                    ServiceId = 1
                }),
                SetBaseProperties(new DiseaseGroupService
                {
                    Id = 2,
                    DiseaseGroupId = 1,
                    ServiceId = 2
                })
            };
        }
    }
}
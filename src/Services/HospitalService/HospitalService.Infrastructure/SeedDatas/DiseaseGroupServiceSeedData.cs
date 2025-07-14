using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class DiseaseGroupServiceSeedData
    {
        private static DiseaseGroupService SetBaseProperties(DiseaseGroupService entity)
        {
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
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

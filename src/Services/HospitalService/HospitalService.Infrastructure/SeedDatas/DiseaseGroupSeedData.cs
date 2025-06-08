using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class DiseaseGroupSeedData
    {
        private static DiseaseGroup SetBaseProperties(DiseaseGroup entity)
        {
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            return entity;
        }

        public static IEnumerable<DiseaseGroup> GetSeedData()
        {
            return new List<DiseaseGroup>
            {
                SetBaseProperties(new DiseaseGroup
                {
                    Id = 1,
                    GroupName = SeedConstants.DiseaseGroups.Infectious,
                    Description = SeedConstants.DiseaseGroups.InfectiousDesc
                }),
                SetBaseProperties(new DiseaseGroup
                {
                    Id = 2,
                    GroupName = SeedConstants.DiseaseGroups.NonInfectious,
                    Description = SeedConstants.DiseaseGroups.NonInfectiousDesc
                })
            };
        }
    }
}

using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class ServiceGroupSeedData
    {
        private static ServiceGroup SetBaseProperties(ServiceGroup entity)
        {
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;

            return entity;
        }

        public static IEnumerable<ServiceGroup> GetSeedData()
        {
            return new List<ServiceGroup>
            {
                SetBaseProperties(new ServiceGroup
                {
                    Id = 1,
                    GroupName = SeedConstants.ServiceGroups.ExamFee
                }),
                SetBaseProperties(new ServiceGroup
                {
                    Id = 2,
                    GroupName = SeedConstants.ServiceGroups.InjectionFee
                }),
                SetBaseProperties(new ServiceGroup
                {
                    Id = 3,
                    GroupName = SeedConstants.ServiceGroups.LaboratoryTest
                })
            };
        }
    }
}

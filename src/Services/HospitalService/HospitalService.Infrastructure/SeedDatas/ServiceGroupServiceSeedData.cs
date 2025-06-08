using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class ServiceGroupServiceSeedData
    {
        private static ServiceGroupService SetBaseProperties(ServiceGroupService entity)
        {
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            return entity;
        }

        public static IEnumerable<ServiceGroupService> GetSeedData()
        {
            return new List<ServiceGroupService>
            {
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 1,
                    ServiceGroupId = 1,
                    ServiceId = 1
                }),
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 2,
                    ServiceGroupId = 2,
                    ServiceId = 2
                })
            };
        }
    }
}

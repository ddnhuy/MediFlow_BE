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
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
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
                }),
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 3,
                    ServiceGroupId = 2,
                    ServiceId = 3
                }),
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 4,
                    ServiceGroupId = 2,
                    ServiceId = 4
                }),
                // Blood Test Service belongs to Laboratory Test group
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 5,
                    ServiceGroupId = 3,
                    ServiceId = 5
                }),
                // Hepatitis B Test Service belongs to Laboratory Test group
                SetBaseProperties(new ServiceGroupService
                {
                    Id = 6,
                    ServiceGroupId = 3,
                    ServiceId = 6
                })
            };
        }
    }
}

using HospitalService.Domain.Models;
using HospitalService.Infrastructure.SeedDatas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiseaseGroup>().HasData(DiseaseGroupSeedData.GetSeedData());
            modelBuilder.Entity<ServiceGroup>().HasData(ServiceGroupSeedData.GetSeedData());
            modelBuilder.Entity<Service>().HasData(ServiceSeedData.GetSeedData());
            modelBuilder.Entity<DiseaseGroupService>().HasData(DiseaseGroupServiceSeedData.GetSeedData());
            modelBuilder.Entity<ServiceGroupService>().HasData(ServiceGroupServiceSeedData.GetSeedData());
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data.SeedDatas;

namespace VaccinationReception.Infrastructure.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceType>().HasData(ServiceTypeSeedData.GetSeedData());
            modelBuilder.Entity<DiseaseGroup>().HasData(DiseaseGroupSeedData.GetSeedData());
            modelBuilder.Entity<ServiceGroup>().HasData(ServiceGroupSeedData.GetSeedData());
            modelBuilder.Entity<Service>().HasData(ServiceSeedData.GetSeedData());
            modelBuilder.Entity<DiseaseGroupService>().HasData(DiseaseGroupServiceSeedData.GetSeedData());
            modelBuilder.Entity<ServiceGroupService>().HasData(ServiceGroupServiceSeedData.GetSeedData());
        }
    } 
}
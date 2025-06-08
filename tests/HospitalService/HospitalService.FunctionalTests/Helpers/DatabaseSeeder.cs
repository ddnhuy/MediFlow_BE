using HospitalService.Domain.Models;
using HospitalService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Helpers
{
    public static class DatabaseSeeder
    {
        public static void SeedTestData(ApplicationDbContext dbContext)
        {
            if (!dbContext.ServiceGroups.Any())
            {
                var serviceGroups = new List<ServiceGroup>
                {
                    new ServiceGroup
                    {
                        GroupName = "Vaccination Services",
                    },
                    new ServiceGroup
                    {
                        GroupName = "Medical Check-up Services",
                    },
                    new ServiceGroup
                    {
                        GroupName = "Laboratory Services",
                    },
                    new ServiceGroup
                    {
                        GroupName = "Imaging Services",
                    }
                };

                 dbContext.ServiceGroups.AddRange(serviceGroups);
                 dbContext.SaveChanges();
            }
        }
    }
}
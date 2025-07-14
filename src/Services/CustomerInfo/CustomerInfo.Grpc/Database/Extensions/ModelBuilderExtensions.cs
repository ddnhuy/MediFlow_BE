using CustomerInfo.Grpc.Models;
using CustomerInfo.Grpc.SeedDatas;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.Grpc.Database.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(PatientSeedData.GetSeedData());
        }
    }
}
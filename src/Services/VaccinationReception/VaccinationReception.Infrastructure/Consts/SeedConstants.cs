using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Infrastructure.Consts
{
    public static class SeedConstants
    {
        public static class BaseProperties
        {
            public static readonly DateTime SeedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            public const int DefaultCreatedBy = 1;
            public const int DefaultLastUpdatedBy = 1;
            public const bool DefaultIsSuspended = false;
            public const bool DefaultIsCancelled = false;
        }

        public static class ServiceTypes
        {
            public const string BasicCode = "VAC001";
            public const string SpecialCode = "VAC002";
            public const string ContractCode = "VAC003";
            public const string BasicName = "Tiêm chủng dịch vụ";
            public const string SpecialName = "Tiêm chủng đặc biệt";
            public const string ContractName = "Tiêm chủng hợp đồng";
        }
    }
}
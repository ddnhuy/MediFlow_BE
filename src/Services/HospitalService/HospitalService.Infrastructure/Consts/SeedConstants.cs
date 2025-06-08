using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.Consts
{
    public class SeedConstants
    {
        public static class BaseProperties
        {
            public static readonly DateTime SeedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            public const int DefaultCreatedBy = 1;
            public const int DefaultLastUpdatedBy = 1;
            public const bool DefaultIsSuspended = false;
            public const bool DefaultIsCancelled = false;
        }

        public static class ServiceGroups
        {
            public const string BasicVaccination = "Nhóm dịch vụ tiêm chủng cơ bản";
            public const string SpecialVaccination = "Nhóm dịch vụ tiêm chủng đặc biệt";
        }

        public static class DiseaseGroups
        {
            public const string Infectious = "Nhóm bệnh truyền nhiễm";
            public const string NonInfectious = "Nhóm bệnh không truyền nhiễm";
            public const string InfectiousDesc = "Các bệnh có khả năng lây truyền từ người sang người";
            public const string NonInfectiousDesc = "Các bệnh không có khả năng lây truyền";
        }

        public static class Services
        {
            public const string Vac5in1 = "Tiêm vắc xin 5 trong 1";
            public const string Vac6in1 = "Tiêm vắc xin 6 trong 1";
            public const string Vac5in1Code = "VAC001";
            public const string Vac6in1Code = "VAC002";
            public const decimal Vac5in1Price = 500000;
            public const decimal Vac6in1Price = 600000;

            public const int VaccinationDepartmentId = 1;
        }
    }
}

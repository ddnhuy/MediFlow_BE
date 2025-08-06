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
            public const string InjectionFee = "Công tiêm";
            public const string ExamFee = "Công khám";
            public const string LaboratoryTest = "Nhóm dịch vụ xét nghiệm";
        }

        public static class DiseaseGroups
        {
            public const string Infectious = "Nhóm bệnh truyền nhiễm";
            public const string NonInfectious = "Nhóm bệnh không truyền nhiễm";
            public const string InfectiousDesc = "Các bệnh có khả năng lây truyền từ người sang người";
            public const string NonInfectiousDesc = "Các bệnh không có khả năng lây truyền";
            public const string PreInjectionScreeningName = "Khám sàng lọc trước tiêm";
            public const string PreInjectionScreeningDesc = "Nhóm bệnh dùng để phân loại các dịch vụ khám sức khỏe nhằm đánh giá tình trạng người bệnh trước khi thực hiện tiêm chủng.";
        }

        public static class Services
        {
            public const string ExamFeeCode = "EXAMFEE";
            public const string ExamFee = "Công khám";
            public const decimal ExamFeePrice = 50000;

            public const string InjectIMCode = "IM";
            public const string InjectIMName = "Công tiêm bắp (IM)";
            public const decimal InjectIMPrice = 30000;

            public const string InjectSCCode = "SC";
            public const string InjectSCName = "Công tiêm dưới da (SC)";
            public const decimal InjectSCPrice = 25000;

            public const string InjectIDCode = "ID";
            public const string InjectIDName = "Công tiêm trong da (ID)";
            public const decimal InjectIDPrice = 35000;

            // Blood Test Service
            public const string BloodTestCode = "BLOOD001";
            public const string BloodTest = "Xét nghiệm công thức máu";
            public const decimal BloodTestPrice = 150000;

            // Hepatitis B Test Service
            public const string HepatitisBTestCode = "HEPB001";
            public const string HepatitisBTest = "Xét nghiệm kháng thể viêm gan B";
            public const decimal HepatitisBTestPrice = 250000;

            public const int VaccinationDepartmentId = 1;
            public const int LaboratoryDepartmentId = 2;
        }
    }
}

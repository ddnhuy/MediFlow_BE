using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class ServiceTestParameterSeedData
    {
        private static ServiceTestParameter SetBaseProperties(ServiceTestParameter entity)
        {
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            return entity;
        }

        public static IEnumerable<ServiceTestParameter> GetSeedData()
        {
            return new List<ServiceTestParameter>
            {
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 1,
                    ServiceId = 7,
                    ParameterName = "WBC (White Blood Cell)",
                    Unit = "G/L",
                    StandardValue = "4.0 - 11.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 2,
                    ServiceId = 7,
                    ParameterName = "LYM (Lymphocyte)",
                    Unit = "%",
                    StandardValue = "20.0 - 40.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 3,
                    ServiceId = 7,
                    ParameterName = "NEU (Neutrophil)",
                    Unit = "%",
                    StandardValue = "50.0 - 70.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 4,
                    ServiceId = 7,
                    ParameterName = "MON (Monocyte)",
                    Unit = "%",
                    StandardValue = "2.0 - 8.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 5,
                    ServiceId = 7,
                    ParameterName = "EOS (Eosinophils)",
                    Unit = "%",
                    StandardValue = "1.0 - 4.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 6,
                    ServiceId = 7,
                    ParameterName = "BASO (Basophils)",
                    Unit = "%",
                    StandardValue = "0.0 - 1.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 7,
                    ServiceId = 7,
                    ParameterName = "RBC (Red Blood Cell)",
                    Unit = "T/L",
                    StandardValue = "4.0 - 5.5",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 8,
                    ServiceId = 7,
                    ParameterName = "HGB (Hemoglobin)",
                    Unit = "g/L",
                    StandardValue = "130 - 175",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 9,
                    ServiceId = 7,
                    ParameterName = "HCT (Hematocrit)",
                    Unit = "%",
                    StandardValue = "40.0 - 50.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 10,
                    ServiceId = 7,
                    ParameterName = "MCV (Mean Corpuscular Volume)",
                    Unit = "fL",
                    StandardValue = "80.0 - 100.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 11,
                    ServiceId = 7,
                    ParameterName = "MCH (Mean Corpuscular Hemoglobin)",
                    Unit = "pg",
                    StandardValue = "27.0 - 32.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 12,
                    ServiceId = 7,
                    ParameterName = "MCHC (Mean Corpuscular Hemoglobin Concentration)",
                    Unit = "g/L",
                    StandardValue = "320 - 360",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 13,
                    ServiceId = 7,
                    ParameterName = "RDW (Red Cell Distribution Width)",
                    Unit = "%",
                    StandardValue = "11.5 - 14.5",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 14,
                    ServiceId = 7,
                    ParameterName = "PLT (Platelet Count)",
                    Unit = "G/L",
                    StandardValue = "150 - 450",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 15,
                    ServiceId = 7,
                    ParameterName = "PCT (Plateletcrit)",
                    Unit = "%",
                    StandardValue = "0.1 - 0.3",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 16,
                    ServiceId = 7,
                    ParameterName = "PDW (Platelet Distribution Width)",
                    Unit = "%",
                    StandardValue = "10.0 - 17.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 17,
                    ServiceId = 7,
                    ParameterName = "MPV (Mean Platelet Volume)",
                    Unit = "fL",
                    StandardValue = "7.0 - 11.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 18,
                    ServiceId = 7,
                    ParameterName = "P-LCR (Plateletcrit Larger Cell Ratio)",
                    Unit = "%",
                    StandardValue = "15.0 - 35.0",
                    EquipmentName = "Máy phân tích huyết học tự động",
                    SpecimenType = "Máu toàn phần",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 19,
                    ServiceId = 8,
                    ParameterName = "HBsAb (Anti-HBs)",
                    Unit = "mIU/mL",
                    StandardValue = "> 10",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 20,
                    ServiceId = 8,
                    ParameterName = "HBsAg",
                    Unit = "IU/mL",
                    StandardValue = "< 0.05",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 21,
                    ServiceId = 8,
                    ParameterName = "HBeAg",
                    Unit = "S/CO",
                    StandardValue = "< 1.0",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 22,
                    ServiceId = 8,
                    ParameterName = "Anti-HBe",
                    Unit = "S/CO",
                    StandardValue = "> 1.0",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 23,
                    ServiceId = 8,
                    ParameterName = "Anti-HBc IgM",
                    Unit = "S/CO",
                    StandardValue = "< 1.0",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 24,
                    ServiceId = 8,
                    ParameterName = "Anti-HBc IgG",
                    Unit = "S/CO",
                    StandardValue = "> 1.0",
                    EquipmentName = "Máy ELISA tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 25,
                    ServiceId = 8,
                    ParameterName = "HBV-DNA",
                    Unit = "IU/mL",
                    StandardValue = "< 20",
                    EquipmentName = "Máy PCR real-time",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 26,
                    ServiceId = 8,
                    ParameterName = "AST (SGOT)",
                    Unit = "U/L",
                    StandardValue = "7 - 40",
                    EquipmentName = "Máy sinh hóa tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 27,
                    ServiceId = 8,
                    ParameterName = "ALT (SGPT)",
                    Unit = "U/L",
                    StandardValue = "7 - 40",
                    EquipmentName = "Máy sinh hóa tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 28,
                    ServiceId = 8,
                    ParameterName = "GGT",
                    Unit = "U/L",
                    StandardValue = "7 - 32",
                    EquipmentName = "Máy sinh hóa tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 29,
                    ServiceId = 8,
                    ParameterName = "Bilirubin toàn phần",
                    Unit = "mg/dL",
                    StandardValue = "0.3 - 1.2",
                    EquipmentName = "Máy sinh hóa tự động",
                    SpecimenType = "Huyết thanh",
                }),
                SetBaseProperties(new ServiceTestParameter
                {
                    Id = 30,
                    ServiceId = 8,
                    ParameterName = "Albumin",
                    Unit = "g/dL",
                    StandardValue = "3.5 - 5.0",
                    EquipmentName = "Máy sinh hóa tự động",
                    SpecimenType = "Huyết thanh",
                }),
            };
        }
    }
}

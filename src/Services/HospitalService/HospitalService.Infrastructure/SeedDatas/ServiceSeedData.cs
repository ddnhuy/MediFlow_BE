using BuildingBlocks.Strings.Enums;
using HospitalService.Domain;
using HospitalService.Domain.Models;
using HospitalService.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Infrastructure.SeedDatas
{
    public static class ServiceSeedData
    {
        private static Service SetBaseProperties(Service entity)
        {
            entity.LastUpdatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.IsCancelled = SeedConstants.BaseProperties.DefaultIsCancelled;
            entity.CreatedBy = SeedConstants.BaseProperties.DefaultCreatedBy;
            entity.IsSuspended = SeedConstants.BaseProperties.DefaultIsSuspended;
            entity.CreatedAt = SeedConstants.BaseProperties.SeedDate;
            entity.LastUpdatedBy = SeedConstants.BaseProperties.DefaultLastUpdatedBy;
            return entity;
        }

        public static IEnumerable<Service> GetSeedData()
        {
            return new List<Service>
            {
                SetBaseProperties(new Service
                {
                    Id = 1,
                    ServiceCode = SeedConstants.Services.ExamFeeCode,
                    ServiceName = SeedConstants.Services.ExamFee,
                    ServiceType = ServiceType.Exam,
                    UnitPrice = SeedConstants.Services.ExamFeePrice,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                }),
                SetBaseProperties(new Service
                {
                    Id = 2,
                    ServiceCode = SeedConstants.Services.InjectIMCode,
                    ServiceName = SeedConstants.Services.InjectIMName,
                    ServiceType = ServiceType.Injection,
                    UnitPrice = SeedConstants.Services.InjectIMPrice,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                }),
                SetBaseProperties(new Service
                {
                    Id = 3,
                    ServiceCode = SeedConstants.Services.InjectSCCode,
                    ServiceName = SeedConstants.Services.InjectSCName,
                    ServiceType = ServiceType.Injection,
                    UnitPrice = SeedConstants.Services.InjectSCPrice,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                }),
                SetBaseProperties(new Service
                {
                    Id = 4,
                    ServiceCode = SeedConstants.Services.InjectIDCode,
                    ServiceName = SeedConstants.Services.InjectIDName,
                    ServiceType = ServiceType.Injection,
                    UnitPrice = SeedConstants.Services.InjectIDPrice,
                    DepartmentId = SeedConstants.Services.VaccinationDepartmentId
                }),
                // Blood Test Service
                SetBaseProperties(new Service
                {
                    Id = 5,
                    ServiceCode = SeedConstants.Services.BloodTestCode,
                    ServiceName = SeedConstants.Services.BloodTest,
                    ServiceType = ServiceType.Test,
                    ExaminationService = BuildingBlocks.Strings.Enums.ExaminationService.Blood,
                    UnitPrice = SeedConstants.Services.BloodTestPrice,
                    DepartmentId = SeedConstants.Services.LaboratoryDepartmentId
                }),
                // Hepatitis B Test Service
                SetBaseProperties(new Service
                {
                    Id = 6,
                    ServiceCode = SeedConstants.Services.HepatitisBTestCode,
                    ServiceName = SeedConstants.Services.HepatitisBTest,
                    ServiceType = ServiceType.Test,
                    ExaminationService = BuildingBlocks.Strings.Enums.ExaminationService.Anti_HBs,
                    UnitPrice = SeedConstants.Services.HepatitisBTestPrice,
                    DepartmentId = SeedConstants.Services.LaboratoryDepartmentId
                }),
            };
        }
    }
}

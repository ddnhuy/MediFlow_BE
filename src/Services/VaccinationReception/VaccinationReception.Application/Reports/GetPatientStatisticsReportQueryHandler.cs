// src/Services/VaccinationReception/VaccinationReception.Application/Reports/GetPatientStatisticsReportQuery.cs
using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Reports
{
    public record GetPatientStatisticsReportQuery(
        DateOnly? FromDate = null,
        DateOnly? ToDate = null) : IQuery<PatientStatisticsReportDTO>;

    public class GetPatientStatisticsReportQueryHandler : IQueryHandler<GetPatientStatisticsReportQuery, PatientStatisticsReportDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProtoServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPatientStatisticsReportQueryHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProtoServiceClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _applicationUserProtoServiceClient = applicationUserProtoServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PatientStatisticsReportDTO> Handle(GetPatientStatisticsReportQuery request, CancellationToken cancellationToken)
        {
            // Set default date range if not provided
            var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = request.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // Get all receptions in the date range to get patient IDs
            var receptions = await _context.Receptions
                .AsNoTracking()
                .Where(r => r.ReceptionDate >= fromDateTime && r.ReceptionDate <= toDateTime
                    && !r.IsSuspended && !r.IsCancelled)
                .Select(r => new { r.PatientId, r.ReceptionDate })
                .ToListAsync(cancellationToken);

            var patientIds = receptions.Select(r => r.PatientId).Distinct().ToList();

            // Get patient information from Patient service
            var patients = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(patientIds, null, cancellationToken);

            // Calculate age groups
            var ageGroupStatistics = CalculateAgeGroupStatistics(patients);

            // Calculate location statistics
            var locationStatistics = CalculateLocationStatistics(patients);

            // Calculate summary
            var summary = new PatientStatisticSummaryDTO
            {
                TotalPatients = patients.Count
            };

            // Get current user info
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _applicationUserProtoServiceClient.GetApplicationUserAsync(
                    new GetApplicationUserRequest { Id = Convert.ToInt32(currentUserId) },
                    metadata);

            return new PatientStatisticsReportDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = currentUser.Name ?? "Admin",
                Summary = summary,
                AgeGroupStatistics = ageGroupStatistics,
                LocationStatistics = locationStatistics
            };
        }

        private List<AgeGroupStatisticDTO> CalculateAgeGroupStatistics(List<PatientSummaryDTO> patients)
        {
            var totalPatients = patients.Count;
            if (totalPatients == 0) return new List<AgeGroupStatisticDTO>();

            var ageGroups = new Dictionary<string, (string Range, int Count)>
            {
                { "Trẻ em", ("0-17 tuổi", 0) },
                { "Người lớn", ("18-59 tuổi", 0) },
                { "Người cao tuổi", ("≥60 tuổi", 0) }
            };

            foreach (var patient in patients)
            {
                var age = CalculateAge(patient.DOB);

                if (age < 18)
                    ageGroups["Trẻ em"] = (ageGroups["Trẻ em"].Range, ageGroups["Trẻ em"].Count + 1);
                else if (age < 60)
                    ageGroups["Người lớn"] = (ageGroups["Người lớn"].Range, ageGroups["Người lớn"].Count + 1);
                else
                    ageGroups["Người cao tuổi"] = (ageGroups["Người cao tuổi"].Range, ageGroups["Người cao tuổi"].Count + 1);
            }

            return ageGroups.Select(kvp => new AgeGroupStatisticDTO
            {
                AgeGroup = kvp.Key,
                AgeRange = kvp.Value.Range,
                PatientCount = kvp.Value.Count,
                Percentage = totalPatients > 0 ? Math.Round((decimal)kvp.Value.Count / totalPatients * 100, 1) : 0
            }).ToList();
        }

        private List<LocationStatisticDTO> CalculateLocationStatistics(List<PatientSummaryDTO> patients)
        {
            var totalPatients = patients.Count;
            if (totalPatients == 0) return new List<LocationStatisticDTO>();

            var locationGroups = patients
                .GroupBy(p => p.Province ?? "Không xác định")
                .Select(g => new LocationStatisticDTO
                {
                    Province = g.Key,
                    PatientCount = g.Count(),
                    Percentage = totalPatients > 0 ? Math.Round((decimal)g.Count() / totalPatients * 100, 1) : 0
                })
                .OrderByDescending(l => l.PatientCount)
                .Take(10) // Top 10 provinces
                .ToList();

            // Add sequential number
            for (int i = 0; i < locationGroups.Count; i++)
            {
                locationGroups[i].Stt = i + 1;
            }

            return locationGroups;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
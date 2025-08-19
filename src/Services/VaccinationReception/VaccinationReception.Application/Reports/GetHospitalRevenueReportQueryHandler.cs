// src/Services/VaccinationReception/VaccinationReception.Application/Reports/GetHospitalRevenueReportQuery.cs
using BuildingBlocks.CQRS;
using BuildingBlocks.Strings.Enums;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.Reports
{
    public record GetHospitalRevenueReportQuery(
        DateOnly? FromDate = null,
        DateOnly? ToDate = null) : IQuery<HospitalRevenueReportDTO>;

    public class GetHospitalRevenueReportQueryHandler : IQueryHandler<GetHospitalRevenueReportQuery, HospitalRevenueReportDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHospitalService _hospitalService;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProtoServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetHospitalRevenueReportQueryHandler(
            IApplicationDbContext context,
            IHospitalService hospitalService,
            ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProtoServiceClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hospitalService = hospitalService;
            _applicationUserProtoServiceClient = applicationUserProtoServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HospitalRevenueReportDTO> Handle(GetHospitalRevenueReportQuery request, CancellationToken cancellationToken)
        {
            // Set default date range if not provided
            var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = request.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // Get all payments within date range
            var payments = await _context.Payments
                .AsNoTracking()
                .Where(p => p.CreatedAt >= fromDateTime && p.CreatedAt <= toDateTime
                    && p.Status == PaymentStatus.Completed
                    && !p.IsSuspended && !p.IsCancelled)
                .Include(p => p.PaymentDetails)
                    .ThenInclude(pd => pd.ServiceRequestDetail)
                .Include(p => p.PaymentDetails)
                    .ThenInclude(pd => pd.ReceptionVaccination)
                .ToListAsync(cancellationToken);

            // Get all service IDs from payment details
            var serviceIds = payments
                .SelectMany(p => p.PaymentDetails)
                .Where(pd => pd.ServiceRequestDetailId.HasValue)
                .Select(pd => pd.ServiceRequestDetail!.ServiceId)
                .Distinct()
                .ToList();

            // Get service information from HospitalService
            var services = serviceIds.Any()
                ? await _hospitalService.GetServicesByIdsAsync(serviceIds, cancellationToken)
                : new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>();

            var serviceDict = services.ToDictionary(s => s.Id, s => s);

            // Process daily revenues
            var dailyRevenues = new List<DailyRevenueDTO>();
            var currentDate = fromDate;

            while (currentDate <= toDate)
            {
                var dayStart = currentDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                var dayEnd = currentDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

                var dayPayments = payments.Where(p => p.CreatedAt >= dayStart && p.CreatedAt <= dayEnd).ToList();

                var dailyRevenue = CalculateDailyRevenue(currentDate, dayPayments, serviceDict);
                dailyRevenues.Add(dailyRevenue);

                currentDate = currentDate.AddDays(1);
            }

            // Calculate summary
            var summary = new HospitalRevenueSummaryDTO
            {
                TotalExamFeeRevenue = dailyRevenues.Sum(d => d.ExamFeeRevenue),
                TotalTestFeeRevenue = dailyRevenues.Sum(d => d.TestFeeRevenue),
                TotalInjectionRevenue = dailyRevenues.Sum(d => d.InjectionRevenue), 
                TotalRevenue = dailyRevenues.Sum(d => d.TotalRevenue),
                TotalExamCount = dailyRevenues.Sum(d => d.ExamCount),
                TotalTestCount = dailyRevenues.Sum(d => d.TestCount),
                TotalInjectionCount = dailyRevenues.Sum(d => d.InjectionCount), 
                AverageDailyRevenue = dailyRevenues.Any() ? dailyRevenues.Average(d => d.TotalRevenue) : 0
            };

            // Get current user info
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _applicationUserProtoServiceClient.GetApplicationUserAsync(
                    new GetApplicationUserRequest { Id = Convert.ToInt32(userId) },
                    metadata);

            return new HospitalRevenueReportDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = currentUser.Name ?? "Admin",
                Summary = summary,
                DailyRevenues = dailyRevenues
            };
        }

        private DailyRevenueDTO CalculateDailyRevenue(
            DateOnly date,
            List<Domain.Models.Payment> dayPayments,
            Dictionary<int, BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO> serviceDict)
        {
            decimal examFeeRevenue = 0;
            decimal testFeeRevenue = 0;
            decimal injectionRevenue = 0;
            int examCount = 0;
            int testCount = 0;
            int injectionCount = 0;

            foreach (var payment in dayPayments)
            {
                foreach (var detail in payment.PaymentDetails.Where(pd => !pd.IsReversed))
                {
                    if (detail.ServiceRequestDetailId.HasValue)
                    {
                        var serviceRequestDetail = detail.ServiceRequestDetail!;
                        var service = serviceDict.GetValueOrDefault(serviceRequestDetail.ServiceId);

                        if (service != null)
                        {
                            // Phân loại dựa trên ServiceCode và ServiceType
                            if (service.ServiceCode == "EXAMFEE")
                            {
                                // Tiền khám
                                examFeeRevenue += detail.Amount;
                                examCount += serviceRequestDetail.Quantity;
                            }
                            else if (service.ExaminationService == ExaminationService.Anti_HBs || service.ExaminationService == ExaminationService.Blood)
                            {
                                // Tiền xét nghiệm
                                testFeeRevenue += detail.Amount;
                                testCount += serviceRequestDetail.Quantity;
                            }
                            else if (service.ServiceCode == "IM" || service.ServiceCode == "SC" || service.ServiceCode == "ID")
                            {
                                // Số công tiêm (Injection services)
                                injectionRevenue += detail.Amount;
                                injectionCount += serviceRequestDetail.Quantity;
                            }
                        }
                    }
                }
            }

            return new DailyRevenueDTO
            {
                Date = date,
                ExamFeeRevenue = examFeeRevenue,
                TestFeeRevenue = testFeeRevenue,
                InjectionRevenue = injectionRevenue, 
                TotalRevenue = examFeeRevenue + testFeeRevenue + injectionRevenue,
                ExamCount = examCount,
                TestCount = testCount,
                InjectionCount = injectionCount 
            };
        }
    }
}
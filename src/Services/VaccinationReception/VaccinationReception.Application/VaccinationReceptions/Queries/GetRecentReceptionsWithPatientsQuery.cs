using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetRecentReceptionsWithPatientsQuery(
        PaginationRequest PaginationRequest,
        string? SearchTerm
    ) : IQuery<GetRecentReceptionsWithPatientsResult>;

    public record GetRecentReceptionsWithPatientsResult(PaginatedResult<RecentReceptionWithPatientDTO> Receptions);

    public class GetRecentReceptionsWithPatientsQueryHandler : IQueryHandler<GetRecentReceptionsWithPatientsQuery, GetRecentReceptionsWithPatientsResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetRecentReceptionsWithPatientsQueryHandler> _logger;

        public GetRecentReceptionsWithPatientsQueryHandler(
            IApplicationDbContext context,
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetRecentReceptionsWithPatientsQueryHandler> logger)
        {
            _context = context;
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<GetRecentReceptionsWithPatientsResult> Handle(
            GetRecentReceptionsWithPatientsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting recent receptions updated within the last 2 hours with pagination: Page {PageIndex}, Size {PageSize}, SearchTerm: {SearchTerm}",
                    request.PaginationRequest.PageIndex, request.PaginationRequest.PageSize, request.SearchTerm);

                var cutoffTime = DateTime.UtcNow.AddHours(-2);

                var recentReceptions = await _context.Receptions
                    .Where(r => r.LastUpdatedAt >= cutoffTime && !r.IsCancelled && !r.IsSuspended && r.IsVaccinationTodayConfirmed == false)
                    .OrderByDescending(r => r.LastUpdatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Found {Count} receptions updated within the last 2 hours", recentReceptions.Count);

                var patientIds = recentReceptions.Select(r => r.PatientId).Distinct().ToList();

                var patientsList = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(
                    patientIds,
                    null,
                    cancellationToken
                );

                var patients = patientsList.ToDictionary(p => p.Id, p => p);

                var result = new List<RecentReceptionWithPatientDTO>();

                foreach (var reception in recentReceptions)
                {
                    if (patients.TryGetValue(reception.PatientId, out var patient))
                    {
                        if (!string.IsNullOrEmpty(request.SearchTerm))
                        {
                            var searchTerm = request.SearchTerm.Trim();
                            var isMatch = patient.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                                          patient.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                                          patient.PhoneNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                                          patient.IdentityCard?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true;

                            if (!isMatch)
                            {
                                continue;
                            }
                        }

                        result.Add(new RecentReceptionWithPatientDTO
                        {
                            ReceptionId = reception.Id,
                            ServiceTypeId = reception.ServiceTypeId,
                            ReceptionDate = reception.ReceptionDate,
                            LastUpdatedAt = reception.LastUpdatedAt,
                            Patient = patient
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Patient information not found for PatientId {PatientId} in ReceptionId {ReceptionId}",
                            reception.PatientId, reception.Id);
                    }
                }

                var totalCount = result.Count;
                var pagedData = result
                    .OrderByDescending(r => r.LastUpdatedAt)
                    .Skip((request.PaginationRequest.PageIndex - 1) * request.PaginationRequest.PageSize)
                    .Take(request.PaginationRequest.PageSize)
                    .ToList();

                var paginatedResult = new PaginatedResult<RecentReceptionWithPatientDTO>(
                    request.PaginationRequest.PageIndex,
                    request.PaginationRequest.PageSize,
                    totalCount,
                    pagedData
                );

                _logger.LogInformation("Successfully processed {Count} receptions with patient information, returned {PagedCount} items for page {PageIndex}",
                    totalCount, pagedData.Count, request.PaginationRequest.PageIndex);

                return new GetRecentReceptionsWithPatientsResult(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving recent receptions with patient information");
                throw;
            }
        }
    }
}
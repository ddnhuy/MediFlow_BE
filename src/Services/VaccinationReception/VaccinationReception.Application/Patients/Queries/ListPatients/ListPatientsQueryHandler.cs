using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Patients.Queries.ListPatients
{
    public class ListPatientsQueryHandler : IQueryHandler<ListPatientsQuery, ListPatientsResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<ListPatientsQueryHandler> _logger;

        public ListPatientsQueryHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<ListPatientsQueryHandler> logger)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<ListPatientsResult> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listing patients with page: {PageNumber}, size: {PageSize}",
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize);

            try
            {
                var result = await _patientGrpcClient.ListPatientsAsync(request.PaginationRequest, cancellationToken);
                _logger.LogInformation("Found {Count} patients", result.TotalItems);
                return new ListPatientsResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing patients");
                throw;
            }
        }
    }
}
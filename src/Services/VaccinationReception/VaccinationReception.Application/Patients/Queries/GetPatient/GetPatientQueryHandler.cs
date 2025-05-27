using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Patients.Queries.GetPatient
{
    public class GetPatientQueryHandler : IQueryHandler<GetPatientQuery, GetPatientResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<GetPatientQueryHandler> _logger;

        public GetPatientQueryHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<GetPatientQueryHandler> logger)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<GetPatientResult> Handle(GetPatientQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting patient with ID: {PatientId}", request.Id);

            try
            {
                var patient = await _patientGrpcClient.GetPatientAsync(request.Id, cancellationToken);
                _logger.LogInformation("Found patient with ID: {PatientId}", request.Id);
                return new GetPatientResult(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient with ID: {PatientId}", request.Id);
                throw;
            }
        }
    }
}
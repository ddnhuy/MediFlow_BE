using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Patients.Commands.CreatePatient
{
    public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, CreatePatientResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<CreatePatientCommandHandler> _logger;

        public CreatePatientCommandHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<CreatePatientCommandHandler> logger)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<CreatePatientResult> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating patient with code: {PatientCode}", request.Code);

            try
            {
                var result = await _patientGrpcClient.CreatePatientAsync(request, cancellationToken);
                _logger.LogInformation("Created patient with ID: {PatientId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient with code: {PatientCode}", request.Code);
                throw;
            }
        }
    }
}
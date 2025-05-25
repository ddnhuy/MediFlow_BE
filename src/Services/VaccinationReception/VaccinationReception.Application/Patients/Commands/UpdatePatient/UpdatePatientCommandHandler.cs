using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Patients.Commands.UpdatePatient
{
    public class UpdatePatientCommandHandler : ICommandHandler<UpdatePatientCommand, UpdatePatientResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<UpdatePatientCommandHandler> _logger;

        public UpdatePatientCommandHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<UpdatePatientCommandHandler> logger)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<UpdatePatientResult> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating patient with ID: {PatientId}", request.Id);

            try
            {
                var result = await _patientGrpcClient.UpdatePatientAsync(request, cancellationToken);
                _logger.LogInformation("Updated patient with ID: {PatientId}", request.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with ID: {PatientId}", request.Id);
                throw;
            }
        }
    }
}
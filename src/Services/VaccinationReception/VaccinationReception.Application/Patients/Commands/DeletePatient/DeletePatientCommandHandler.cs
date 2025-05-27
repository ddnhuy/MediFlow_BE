using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Patients.Commands.DeletePatient
{
    public class DeletePatientCommandHandler : ICommandHandler<DeletePatientCommand, DeletePatientResult>
    {
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly ILogger<DeletePatientCommandHandler> _logger;

        public DeletePatientCommandHandler(
            IPatientGrpcClient patientGrpcClient,
            ILogger<DeletePatientCommandHandler> logger)
        {
            _patientGrpcClient = patientGrpcClient;
            _logger = logger;
        }

        public async Task<DeletePatientResult> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting patient with ID: {PatientId}", request.Id);

            try
            {
                var result = await _patientGrpcClient.DeletePatientAsync(request.Id, cancellationToken);
                _logger.LogInformation("Deleted patient with ID: {PatientId}", request.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient with ID: {PatientId}", request.Id);
                throw;
            }
        }
    }
}
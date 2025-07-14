using BuildingBlocks.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Helpers;

namespace VaccinationReception.Application.Patients.Queries
{
    public record GeneratePatientIdentifierQuery : IQuery<GeneratePatientIdentifierResult>;

    public record GeneratePatientIdentifierResult(string PatientIdentifier);

    public class GeneratePatientIdentifierQueryHandler : IQueryHandler<GeneratePatientIdentifierQuery, GeneratePatientIdentifierResult>
    {
        private readonly ILogger<GeneratePatientIdentifierQueryHandler> _logger;

        public GeneratePatientIdentifierQueryHandler(ILogger<GeneratePatientIdentifierQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratePatientIdentifierResult> Handle(GeneratePatientIdentifierQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating new patient identifier");

            var patientIdentifier = await UniqueStringGenerator.GeneratePatientIdentifierAsync();

            _logger.LogInformation("Generated patient identifier: {PatientIdentifier}", patientIdentifier);

            return new GeneratePatientIdentifierResult(patientIdentifier);
        }
    }
}

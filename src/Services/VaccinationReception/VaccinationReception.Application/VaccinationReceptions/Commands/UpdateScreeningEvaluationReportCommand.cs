using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record UpdateScreeningEvaluationReportCommand(
            int Id,
            string? ParentFullName,
            string? ParentPhoneNumber,
            double WeightKg,
            double BodyTemperatureC,
            int BloodPressureSystolic,
            int BloodPressureDiastolic,
            bool HasSevereFeverAfterPreviousVaccination,
            bool HasAcuteOrChronicDisease,
            bool IsOnOrRecentlyEndedCorticosteroids,
            bool HasAbnormalTemperatureOrVitals,
            bool HasAbnormalHeartSound,
            bool HasHeartValveDisorder,
            bool HasNeurologicalAbnormalities,
            bool IsUnderweightBelow2000g,
            bool HasOtherContraindications,
            bool IsEligibleForVaccination,
            bool IsContraindicatedForVaccination,
            bool IsVaccinationDeferred,
            bool IsReferredToHospital,
            int ReceptionId
        ) : ICommand<UpdateScreeningEvaluationReportResult>;

    public record UpdateScreeningEvaluationReportResult(bool IsSuccess);
}
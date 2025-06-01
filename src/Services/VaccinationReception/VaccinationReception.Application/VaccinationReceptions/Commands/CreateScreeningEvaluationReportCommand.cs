using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{

    public record CreateScreeningEvaluationReportCommand(
        string ParentFullName,
        string ParentPhoneNumber,

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
     ) : ICommand<CreateScreeningEvaluationReportResult>;

    public record CreateScreeningEvaluationReportResult(int screeningEvaluationReportId);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class ScreeningEvaluationReportDTO
    {
        public int Id { get; set; }
        public int ReceptionId { get; set; }

        public string? ParentPhoneNumber { get; set; }
        public string? ParentFullName { get; set; }


        public double WeightKg { get; set; }
        public double BodyTemperatureC { get; set; }
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }

        public bool IsOnOrRecentlyEndedCorticosteroids { get; set; }
        public bool HasAbnormalTemperatureOrVitals { get; set; }
        public bool HasSevereFeverAfterPreviousVaccination { get; set; }
        public bool HasHeartValveDisorder { get; set; }
        public bool HasAcuteOrChronicDisease { get; set; }
        public bool HasAbnormalHeartSound { get; set; }
        public bool HasImmunodeficiencyOrSuspectedHiv { get; set; }
        public bool HasNeurologicalAbnormalities { get; set; }
        public bool HasOtherContraindications { get; set; }

        public bool HasPaleSkinOrLips { get; set; }
        public bool IsPretermBelow34Weeks { get; set; }
        public bool HasAbnormalCry { get; set; }
        public bool IsUnderweightBelow2000g { get; set; }
        public bool HasPoorFeeding { get; set; }

        public bool IsContraindicatedForVaccination { get; set; }
        public bool IsEligibleForVaccination { get; set; }
        public bool IsReferredToHospital { get; set; }
        public bool IsVaccinationDeferred { get; set; }

        public int CreatedBy { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class CreateScreeningEvaluationReportDTO
    {
        public string ParentFullName { get; set; } = string.Empty;
        public string ParentPhoneNumber { get; set; } = string.Empty;

        // Additional screening information
        public double WeightKg { get; set; }
        public double BodyTemperatureC { get; set; }
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }

        // Boolean screening items
        public bool HasSevereFeverAfterPreviousVaccination { get; set; }
        public bool HasAcuteOrChronicDisease { get; set; }
        public bool IsOnOrRecentlyEndedCorticosteroids { get; set; }
        public bool HasAbnormalTemperatureOrVitals { get; set; }
        public bool HasAbnormalHeartSound { get; set; }
        public bool HasHeartValveDisorder { get; set; }
        public bool HasNeurologicalAbnormalities { get; set; }
        public bool IsUnderweightBelow2000g { get; set; }
        public bool HasOtherContraindications { get; set; }

        // Screening results
        public bool IsEligibleForVaccination { get; set; }
        public bool IsContraindicatedForVaccination { get; set; }
        public bool IsVaccinationDeferred { get; set; }
        public bool IsReferredToHospital { get; set; }

        // Link to Reception
        public int ReceptionId { get; set; }
    }
}
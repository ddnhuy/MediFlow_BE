using System;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ScreeningEvaluationReport : BaseEntity
    {
        // Parent information
        public string? ParentFullName { get; set; } = string.Empty;
        public string? ParentPhoneNumber { get; set; } = string.Empty;

        // Additional screening information
        public double WeightKg { get; set; }                          // Weight, e.g., 50 (kg)
        public double BodyTemperatureC { get; set; }                  // Body temperature, e.g., 100 (°C)
        public int BloodPressureSystolic { get; set; }                // Systolic blood pressure, e.g., 120 (mmHg)
        public int BloodPressureDiastolic { get; set; }               // Diastolic blood pressure, e.g., 80 (mmHg)

        // Boolean screening items
        public bool HasSevereFeverAfterPreviousVaccination { get; set; }      // Item 1
        public bool HasAcuteOrChronicDisease { get; set; }                    // Item 2
        public bool IsOnOrRecentlyEndedCorticosteroids { get; set; }          // Item 3
        public bool HasAbnormalTemperatureOrVitals { get; set; }              // Item 4
        public bool HasAbnormalHeartSound { get; set; }                       // Item 5
        public bool HasHeartValveDisorder { get; set; }                       // Item 6
        public bool HasNeurologicalAbnormalities { get; set; }                // Item 7
        public bool IsUnderweightBelow2000g { get; set; }                     // Item 8
        public bool HasOtherContraindications { get; set; }                   // Item 9

        // Screening results
        public bool IsEligibleForVaccination { get; set; }
        public bool IsContraindicatedForVaccination { get; set; }
        public bool IsVaccinationDeferred { get; set; }
        public bool IsReferredToHospital { get; set; }

        // Link to Reception
        public int ReceptionId { get; set; }
        public Reception? Reception { get; set; }
    }
}
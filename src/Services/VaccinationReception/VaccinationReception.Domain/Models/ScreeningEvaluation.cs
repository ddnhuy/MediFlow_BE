using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ScreeningEvaluation : BaseEntity
    {
        public bool HasSevereFeverAfterPreviousVaccination { get; set; }      // Item 1
        public bool HasAcuteOrChronicDisease { get; set; }                    // Item 2
        public bool IsOnOrRecentlyEndedCorticosteroids { get; set; }          // Item 3
        public bool HasAbnormalTemperatureOrVitals { get; set; }              // Item 4
        public bool HasAbnormalHeartSound { get; set; }                       // Item 5
        public bool HasHeartValveDisorder { get; set; }                       // Item 6
        public bool HasNeurologicalAbnormalities { get; set; }                // Item 7
        public bool IsUnderweightBelow2000g { get; set; }                     // Item 8
        public bool HasOtherContraindications { get; set; }                   // Item 9

        public bool IsEligibleForVaccination { get; set; }
        public bool IsContraindicatedForVaccination { get; set; }
        public bool IsVaccinationDeferred { get; set; }
        public bool IsReferredToHospital { get; set; }

        public int ReceptionId { get; set; }
        public Reception? Reception { get; set; }
    }
}
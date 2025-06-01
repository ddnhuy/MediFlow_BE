using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.PatientDTOs
{
    public class CreatePatientDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public string AddressDetail { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsForeigner { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}
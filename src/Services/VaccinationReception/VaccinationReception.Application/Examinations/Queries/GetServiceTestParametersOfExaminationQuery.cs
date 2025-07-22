using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Contracts.HospitalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetServiceTestParametersOfExaminationQuery(int ExaminationId) : IQuery<GetServiceTestParametersOfExaminationResponse>;

    public record GetServiceTestParametersOfExaminationResponse
    {
        public List<ServiceTestParameterDTO> ServiceTestParameters { get; set; } = new List<ServiceTestParameterDTO>();
    }

    public record ServiceTestParameterDTO
    {
        public string? RequestNumber { get; set; } = string.Empty;
        public string? ParameterName { get; set; } = string.Empty!;
        public string? Result { get; set; } = string.Empty;
        public string? StandardValue { get; set; } = string.Empty;
        public string? Unit { get; set; } = string.Empty;
        public string? SpecimenType { get; set; } = string.Empty;
        public string? EquipmentName { get; set; } = string.Empty;
    }
}

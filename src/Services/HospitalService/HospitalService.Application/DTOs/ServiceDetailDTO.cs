using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.DTOs
{
    public record ServiceDetailDTO(
        int Id,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId,
        //string Unit,
        //string StandardValue,
        //int Quantity,
        //string EquipmentUsed,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        List<ServiceGroupSummaryDTO> ServiceGroups,
        List<DiseaseGroupSummaryDTO> DiseaseGroups
    );

    public record ServiceGroupSummaryDTO(
        int Id,
        string GroupName
    );

    public record DiseaseGroupSummaryDTO(
        int Id,
        string GroupName,
        string? Description
    );
}
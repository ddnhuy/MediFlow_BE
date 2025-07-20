using BuildingBlocks.Strings.Enums;
using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.DTOs
{
    public record ServiceDTO(
        int Id,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId,
        ExaminationService? ExaminationService,
        ICollection<ServiceTestParameter>? ServiceTestParameters = null!
    );
}

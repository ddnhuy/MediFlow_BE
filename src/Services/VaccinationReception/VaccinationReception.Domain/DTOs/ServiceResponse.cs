using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.DTOs
{
    public record ServiceResponse(
        int Id,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId
    );
}

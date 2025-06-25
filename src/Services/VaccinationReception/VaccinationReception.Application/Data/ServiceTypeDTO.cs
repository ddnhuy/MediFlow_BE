using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Data
{
    public record ServiceTypeDTO(
        int Id,
        string Code,
        string Name,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}

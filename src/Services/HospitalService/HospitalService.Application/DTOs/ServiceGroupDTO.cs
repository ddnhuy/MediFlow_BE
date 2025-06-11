using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.DTOs
{
    public record ServiceGroupDTO(
        int Id,
        string GroupName
    );
}
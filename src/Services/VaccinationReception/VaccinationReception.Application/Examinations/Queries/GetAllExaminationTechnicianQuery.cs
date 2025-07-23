using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetAllExaminationTechnicianQuery(string RoleName) : IQuery<GetAllExaminationTechnicianRespone>;
    
    public record GetAllExaminationTechnicianRespone(
        List<ExaminationTechnicianItem> ExaminationTechnicians
    );

    public record ExaminationTechnicianItem(
        int Id,
        string Name
    );
}

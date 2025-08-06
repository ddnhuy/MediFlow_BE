using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum ContractStatus
    {
        Draft = 0,
        Active = 1,
        Completed = 2,
        Finalized = 3,
        Cancelled = 4
    }
}

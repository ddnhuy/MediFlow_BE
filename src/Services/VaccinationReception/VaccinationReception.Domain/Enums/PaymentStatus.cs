using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum PaymentStatus
    {
        Completed = 0,
        Cancelled = 1,
        Adjusted = 2,
        Pending = 3
    }
}

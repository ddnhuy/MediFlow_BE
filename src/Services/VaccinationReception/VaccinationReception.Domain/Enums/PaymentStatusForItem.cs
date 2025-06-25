using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum PaymentStatusForItem
    {
        NotPaid = 0,
        Paid = 1,
        Refunded = 2,
        AdjustedOut = 3
    }
}

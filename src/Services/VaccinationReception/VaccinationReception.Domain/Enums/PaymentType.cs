using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum PaymentType
    {
        Receipt = 0,     // Phiếu thu
        Refund = 1,      // Hoàn tiền
        Adjustment = 2   // Điều chỉnh
    }

}

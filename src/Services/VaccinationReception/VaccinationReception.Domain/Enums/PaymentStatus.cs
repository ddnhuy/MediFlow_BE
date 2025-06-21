using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum PaymentStatus
    {
        Completed = 0,   // Đã hoàn tất
        Cancelled = 1,   // Đã hủy
        Adjusted = 2     // Đã được điều chỉnh
    }
}

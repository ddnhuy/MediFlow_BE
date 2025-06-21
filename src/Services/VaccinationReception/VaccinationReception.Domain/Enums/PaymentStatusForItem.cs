using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Domain.Enums
{
    public enum PaymentStatusForItem
    {
        NotPaid = 0,        // Chưa thanh toán
        Paid = 1,           // Đã thanh toán
        Refunded = 2,       // Đã thanh toán và hoàn tiền
        AdjustedOut = 3     // Đã bị loại khỏi thanh toán qua điều chỉnh
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Helpers
{
    public static class InvoiceNumberHelper
    {
        public static string RestoreInvoiceNumber(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return description;

            if (description.Contains("-"))
                return description;

            return description.Substring(0, 3) + "-" + description.Substring(3);
        }
    }

}

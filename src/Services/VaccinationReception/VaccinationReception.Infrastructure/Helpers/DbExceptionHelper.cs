using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Infrastructure.Helpers
{
    public static class DbExceptionHelper
    {
        public static bool IsDuplicateKeyException(DbUpdateException exception)
        {
            return exception.InnerException?.Message?.Contains("duplicate key value") == true;
        }
    }
}
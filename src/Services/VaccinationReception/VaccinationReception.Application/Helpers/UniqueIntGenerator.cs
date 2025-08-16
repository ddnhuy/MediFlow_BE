using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Helpers
{
    public static class UniqueIntGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();
        private static readonly HashSet<int> _usedInts = new HashSet<int>();

        public static int GenerateUniqueOrderId()
        {
            lock (_lock)
            {
                int result;
                do
                {
                    int millisOfDay = (int)(DateTime.UtcNow.TimeOfDay.TotalMilliseconds % int.MaxValue);

                    int randomPart = _random.Next(1000, 9999);

                    result = (int)((millisOfDay * 10000L + randomPart) % int.MaxValue);

                } while (_usedInts.Contains(result));

                _usedInts.Add(result);
                return result;
            }
        }

        public static async Task<int> GenerateUniqueOrderIdAsync()
        {
            return await Task.Run(() => GenerateUniqueOrderId());
        }
    }

}

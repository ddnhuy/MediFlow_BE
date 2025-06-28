using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Helpers
{
    public static class UniqueStringGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();
        private static readonly HashSet<string> _usedStrings = new HashSet<string>();

        public static string GenerateUniqueString()
        {
            lock (_lock)
            {
                string result;
                do
                {
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        .ToString().Substring(7, 6);

                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var randomString = new string(Enumerable.Repeat(chars, 5)
                        .Select(s => s[_random.Next(s.Length)]).ToArray());

                    result = $"R{timestamp}{randomString}";

                } while (_usedStrings.Contains(result));

                _usedStrings.Add(result);
                return result;
            }
        }

        public static async Task<string> GenerateUniqueStringAsync()
        {
            return await Task.Run(() => GenerateUniqueString());
        }

        public static string GenerateUniqueStringWithPrefix(string prefix)
        {
            lock (_lock)
            {
                string result;
                do
                {
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var randomString = new string(Enumerable.Repeat(chars, 5)
                        .Select(s => s[_random.Next(s.Length)]).ToArray());

                    result = $"{prefix}-{timestamp}{randomString}";
                } while (_usedStrings.Contains(result));

                _usedStrings.Add(result);
                return result;
            }
        }

        public static string GeneratePatientIdentifier()
        {
            lock (_lock)
            {
                string result;
                do
                {
                    var now = DateTime.Now;

                    // Format: CDCDN + YY + MM + DD + HH + MM + SS + mmm
                    var year = now.ToString("yy");
                    var month = now.ToString("MM");
                    var day = now.ToString("dd");
                    var hour = now.ToString("HH");
                    var minute = now.ToString("mm");
                    var second = now.ToString("ss");
                    var millisecond = now.ToString("fff");

                    result = $"CDCDN{year}{month}{day}{hour}{minute}{second}{millisecond}";

                } while (_usedStrings.Contains(result));

                _usedStrings.Add(result);
                return result;
            }
        }

        public static async Task<string> GeneratePatientIdentifierAsync()
        {
            return await Task.Run(() => GeneratePatientIdentifier());
        }
        public static string GenerateInvoiceNumber()
        {
            lock (_lock)
            {
                string result;
                do
                {
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var randomString = new string(Enumerable.Repeat(chars, 4)
                        .Select(s => s[_random.Next(s.Length)]).ToArray());

                    result = $"IVN-{timestamp}{randomString}";
                } while (_usedStrings.Contains(result));

                _usedStrings.Add(result);
                return result;
            }
        }
        public static async Task<string> GenerateInvoiceNumberAsync()
        {
            return await Task.Run(() => GenerateInvoiceNumber());
        }
    }
}
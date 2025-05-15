using System.Security.Cryptography;

namespace HumanResource.Grpc.Helpers
{
    public static class PasswordGenerator
    {
        public static string GenerateSecurePassword(int length = 12)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@$?_-";

            var randomChars = new[]
            {
                upper[RandomNumberGenerator.GetInt32(upper.Length)],
                lower[RandomNumberGenerator.GetInt32(lower.Length)],
                digits[RandomNumberGenerator.GetInt32(digits.Length)],
                special[RandomNumberGenerator.GetInt32(special.Length)]
            }.ToList();

            string allChars = upper + lower + digits + special;
            while (randomChars.Count < length)
            {
                randomChars.Add(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
            }

            return new string(randomChars.OrderBy(_ => RandomNumberGenerator.GetInt32(100)).ToArray());
        }
    }
}

using System.Security.Cryptography;

namespace Inventory.Application.UniqueCode.Queries
{
    public class GenerateUniqueCodeQueryHandler : IQueryHandler<GenerateUniqueCodeQuery, GenerateUniqueCodeResult>
    {
        /// <summary>
        /// Generate a unique code with format: CDCDN_RT_XXXXXXXX
        /// Using timestamp and random bytes to ensure uniqueness
        /// Combine timestamp and random bytes to create a unique 8-character string
        /// Ensure it's exactly 8 characters by taking the last 8 characters
        /// </summary>
        /// <returns></returns>
        public async Task<GenerateUniqueCodeResult> Handle(GenerateUniqueCodeQuery request, CancellationToken cancellationToken)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var combinedValue = timestamp ^ BitConverter.ToUInt32(randomBytes, 0);
            var uniquePart = Convert.ToString(combinedValue, 16).PadLeft(8, '0').ToUpper();

            if (uniquePart.Length > 8)
            {
                uniquePart = uniquePart.Substring(uniquePart.Length - 8);
            }

            var uniqueCode = $"CDCDN_RT_{uniquePart}";

            return new GenerateUniqueCodeResult(uniqueCode);
        }
    }
}
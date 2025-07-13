namespace Inventory.Application.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Converts an enum value to its string name (e.g., "IM").
        /// Returns null if the value is null.
        /// </summary>
        public static string? ToEnumString<TEnum>(TEnum? value) where TEnum : struct, Enum
        {
            return value?.ToString();
        }
    }
}
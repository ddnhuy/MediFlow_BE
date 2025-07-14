namespace Inventory.Application.DTOs
{
    public class CountryDTO
    {
        public int Id { get; set; }
        public string CountryName { get; set; } = default!;
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}

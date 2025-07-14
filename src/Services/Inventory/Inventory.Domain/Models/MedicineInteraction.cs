namespace Inventory.Domain.Models
{
    public class MedicineInteraction : Entity
    {
        public int MedicineId1 { get; set; }
        public int MedicineId2 { get; set; }
        public string? HarmfulEffects { get; set; } = string.Empty;
        public string? Mechanism { get; set; } = string.Empty;
        public string? PreventiveActions { get; set; } = string.Empty;
        public string? ReferenceInfo { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;

        public Medicine? Medicine1 { get; set; } = default!;
        public Medicine? Medicine2 { get; set; } = default!;
    }
}

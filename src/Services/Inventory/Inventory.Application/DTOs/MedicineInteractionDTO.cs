namespace Inventory.Application.DTOs
{
    public class MedicineInteractionDTO
    {
        public int Id { get; set; }
        public int MedicineId1 { get; set; }
        public string MedicineName1 { get; set; } = string.Empty;
        public int MedicineId2 { get; set; }
        public string MedicineName2 { get; set; } = string.Empty;   
        public string? HarmfulEffects { get; set; }
        public string? Mechanism { get; set; }
        public string? PreventiveActions { get; set; }
        public string? ReferenceInfo { get; set; }
        public string? Notes { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}

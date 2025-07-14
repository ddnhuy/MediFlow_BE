using System.ComponentModel.DataAnnotations;

namespace Appointment.API.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS_DDAC.Models
{
    [Table("Services")]
    public class ServiceModel
    {
        [Key]
        public int ServiceId { get; set; }

        [Required, ForeignKey(nameof(Tenant))]
        public required string TenantId { get; set; }
        public UserModel Tenant { get; set; } = null!;

        [Required, ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public PropertyModel Property { get; set; } = null!;

        [Required, MaxLength(50)]
        public string ServiceType { get; set; } = null!;
        // e.g. "Cleaning"

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Requested";
        // e.g. "Requested", "Scheduled", "Completed"
    }
}

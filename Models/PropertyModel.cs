using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS_DDAC.Models
{
    [Table("Properties")]
    public class PropertyModel
    {
        [Key]
        public int PropertyId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(300)]
        public string Address { get; set; } = null!;

        // 🔹 Change OwnerId to string
        [Required, ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; } = null!;
        public UserModel Owner { get; set; } = null!;

        [Required, MaxLength(10)]
        public string RentalMode { get; set; } = "Unit"; // "Unit" or "Room"

        // 🔹 Change TenantId to string (if using IdentityUser)
        [ForeignKey(nameof(Tenant))]
        public string? TenantId { get; set; }
        public UserModel? Tenant { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}

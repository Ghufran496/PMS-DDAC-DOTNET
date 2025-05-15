using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS_DDAC.Models
{
    [Table("Rooms")]
    public class RoomModel
    {
        [Key]
        public int RoomId { get; set; }

        [Required, ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public PropertyModel Property { get; set; } = null!;

        [Required, MaxLength(50)]
        public string RoomName { get; set; } = null!;

        // Occupant if in "Room" mode
        [ForeignKey(nameof(Tenant))]
        public string? TenantId { get; set; } // 🔹 Change to string
        public UserModel? Tenant { get; set; }
    }
}

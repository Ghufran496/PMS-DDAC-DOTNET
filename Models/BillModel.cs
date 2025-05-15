using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS_DDAC.Models
{
    [Table("Bills")]
    public class BillModel
    {
        [Key]
        public int BillId { get; set; }

        // Still link to a Tenant:
        [Required, ForeignKey(nameof(Tenant))]
        public string TenantId { get; set; } = null!;
        public UserModel Tenant { get; set; } = null!;

        // Make PropertyId nullable, because we might only be billing a Room
        [ForeignKey(nameof(Property))]
        public int? PropertyId { get; set; }
        public PropertyModel? Property { get; set; }

        // Add RoomId for room-based billing
        [ForeignKey(nameof(Room))]
        public int? RoomId { get; set; }
        public RoomModel? Room { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Unpaid";

        // New field to label the type of this bill
        [Required, MaxLength(50)]
        public string BillType { get; set; } = "Rent";
        // e.g. "Rent", "Utility", "Maintenance"

        public string? ReceiptUrl { get; set; }  // Nullable because it's only set after payment

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

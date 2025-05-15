using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMS_DDAC.Models.ViewModels
{
    public class PropertySetupViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string RentalMode { get; set; } = "Unit"; // Unit or Room

        public List<string>? RoomNames { get; set; } = new List<string>();

        public List<PropertyModel> ExistingProperties { get; set; } = new();
        public Dictionary<int, int> PropertyRoomCounts { get; set; } = new();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PMS_DDAC.Models.ViewModels
{
    public class AssignPropertyViewModel
    {
        [Required]
        public string RentalMode { get; set; } // "Room" or "Unit"

        [Required]
        public int SelectedPropertyId { get; set; }

        public List<SelectListItem> Properties { get; set; } = new();

        [Required]
        public string SelectedTenantId { get; set; } // Stores the selected tenant's ID

        public List<string> SelectedTenantEmails { get; set; }

        public List<SelectListItem> Tenants { get; set; } = new(); // Holds available tenants

        public int? SelectedRoomId { get; set; } // Nullable for "Unit" mode
        public List<SelectListItem> Rooms { get; set; } = new(); // Holds available rooms
    }
}

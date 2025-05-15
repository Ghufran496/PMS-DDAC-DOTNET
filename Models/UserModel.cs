using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PMS_DDAC.Models
{
    public class UserModel : IdentityUser  // 🔹 Id is already a string
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;
    }
}

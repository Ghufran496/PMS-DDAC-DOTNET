using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PMS_DDAC.Models.ViewModels
{
    public class TenantCompositeViewModel
    {
        [ValidateNever]
        public TenantProfileViewModel Profile { get; set; } = new TenantProfileViewModel();

        [ValidateNever]
        public ChangePasswordViewModel Password { get; set; } = new ChangePasswordViewModel();

        [ValidateNever]
        public ActivateOwnerViewModel Owner { get; set; } = new ActivateOwnerViewModel();
    }
    public class TenantProfileViewModel
    {
        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = string.Empty; // ❗ Must exist if your Razor uses asp-for="Email"

        [Required, Phone, Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password), Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("NewPassword"), Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } = null!;
    }

    public class ActivateOwnerViewModel
    {
        [Required, Display(Name = "Activation Code")]
        public string ActivationCode { get; set; } = null!;
    }
}

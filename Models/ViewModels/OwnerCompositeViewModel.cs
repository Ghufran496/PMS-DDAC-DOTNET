using System.ComponentModel.DataAnnotations;

namespace PMS_DDAC.Models.ViewModels
{
    public class OwnerCompositeViewModel
    {
        public OwnerProfileViewModel Profile { get; set; } = new();
        public ChangeOwnerPasswordViewModel Password { get; set; } = new();
    }

    public class OwnerProfileViewModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ChangeOwnerPasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

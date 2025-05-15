using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS_DDAC.Models;
using PMS_DDAC.Models.ViewModels;

namespace PMS_DDAC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OwnerProfileController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public OwnerProfileController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var compositeModel = new OwnerCompositeViewModel
            {
                Profile = new OwnerProfileViewModel
                {
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty
                },
                Password = new ChangeOwnerPasswordViewModel()
            };

            return View(compositeModel);
        }

        // POST: Update Owner Profile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(OwnerCompositeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            user.FullName = model.Profile.FullName;
            user.PhoneNumber = model.Profile.PhoneNumber;

            if (user.Email != model.Profile.Email) // Email is changing
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Profile.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, model.Profile.Email, token);

                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors) ModelState.AddModelError("", error.Description);
                    return View("Index", model);
                }

                user.UserName = model.Profile.Email;
                user.NormalizedUserName = model.Profile.Email.ToUpper();
                user.NormalizedEmail = model.Profile.Email.ToUpper();

                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: Change Password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(OwnerCompositeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.Password.CurrentPassword, model.Password.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                return View("Index", model);
            }

            TempData["Success"] = "Password changed successfully!";
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index");
        }

        // 🔥 POST: Delete Account with Confirmation
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Log out before deleting account
            await _signInManager.SignOutAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Your account has been successfully deleted.";
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeToTenantRole()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Remove Owner role
            if (await _userManager.IsInRoleAsync(user, "Owner"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Owner");
            }

            // Assign Tenant role
            if (!await _userManager.IsInRoleAsync(user, "Tenant"))
            {
                await _userManager.AddToRoleAsync(user, "Tenant");
            }

            TempData["Success"] = "You have been switched back to Tenant role for testing.";
            return RedirectToAction("Index");
        }
    }
}

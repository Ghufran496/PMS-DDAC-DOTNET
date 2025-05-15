using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS_DDAC.Models;
using PMS_DDAC.Models.ViewModels;
using System.Threading.Tasks;

namespace PMS_DDAC.Areas.User.Controllers
{
    [Area("User")]
    public class TenantProfileController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public TenantProfileController(UserManager<UserModel> userManager,
                                       SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private async Task<TenantCompositeViewModel> BuildCompositeAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new TenantCompositeViewModel();
            }

            return new TenantCompositeViewModel
            {
                Profile = new TenantProfileViewModel
                {
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty
                },
                Password = new ChangePasswordViewModel(),
                Owner = new ActivateOwnerViewModel()
            };
        }

        // 1. GET: /User/TenantProfile/Index
        //    Returns a fully initialized composite model
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // ✅ Force refresh of authentication session if user is null
            if (user == null)
            {
                var refreshedUser = await _userManager.GetUserAsync(HttpContext.User);
                if (refreshedUser != null)
                {
                    await _signInManager.RefreshSignInAsync(refreshedUser);
                    await Task.Delay(500); // Wait for 500ms before retrying
                    user = await _userManager.GetUserAsync(User);
                }

                if (user == null)
                {
                    TempData["Error"] = "User not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }
            }

            // Build a composite for the page
            var composite = await BuildCompositeAsync();
            return View(composite);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TenantProfileViewModel model)
        {
            // 🔹 Log received values for debugging
            Console.WriteLine($"Received FullName: {model.FullName}");
            Console.WriteLine($"Received Email: {model.Email}");
            Console.WriteLine($"Received PhoneNumber: {model.PhoneNumber}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                TempData["Error"] = "Invalid input.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("Index", model);
            }

            // 🔹 Handle email update separately
            if (user.Email != model.Email) // Email is changing
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, model.Email, token);

                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View("Index", model);
                }

                // ✅ Ensure UserName and NormalizedEmail are updated
                user.UserName = model.Email;  // Update UserName to match email
                user.NormalizedUserName = model.Email.ToUpper(); // Update NormalizedUserName
                user.NormalizedEmail = model.Email.ToUpper(); // Ensure NormalizedEmail is also updated

                await _userManager.UpdateAsync(user);

                TempData["Success"] = "Profile updated! Your email has changed.";
            }
            else
            {
                TempData["Success"] = "Profile updated successfully!";
            }

            return RedirectToAction("Index");
        }



        // 3. POST: Change Password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(TenantCompositeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // 🔹 Log received password values for debugging
            Console.WriteLine($"Attempting password change for user: {user.Email}");
            Console.WriteLine($"Current Password: {model.Password.CurrentPassword}");
            Console.WriteLine($"New Password: {model.Password.NewPassword}");
            Console.WriteLine($"Confirm New Password: {model.Password.ConfirmNewPassword}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                TempData["Error"] = "Invalid input.";
                return RedirectToAction("Index");
            }

            // 🔹 Try changing the password
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.Password.CurrentPassword,
                model.Password.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Password Change Error: {error.Description}");
                    ModelState.AddModelError("", error.Description);
                }

                return View("Index", model); // Return same page with errors
            }

            TempData["Success"] = "Password changed successfully!";
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index");
        }



        // 4. POST: Activate as Owner
        [HttpPost]
        public async Task<IActionResult> ActivateOwner([Bind(Prefix = "Owner")] ActivateOwnerViewModel ownerModel)
        {
            // 1) Manually validate only the ownerModel
            ModelState.Clear();
            if (!TryValidateModel(ownerModel))
            {
                var composite = await BuildCompositeAsync();
                composite.Owner = ownerModel;
                return View("Index", composite);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var owners = await _userManager.GetUsersInRoleAsync("Owner");
            if (owners.Count >= 1)
            {
                TempData["Error"] = "Owner limit reached. The free version allows only one owner.";
                return RedirectToAction("Index");
            }

            if (ownerModel.ActivationCode != "OWNER2024")
            {
                TempData["Error"] = "Invalid activation code.";
                return RedirectToAction("Index");
            }

            // Assign "Owner" role
            await _userManager.AddToRoleAsync(user, "Owner");

            // 🔹 Refresh sign-in to apply role changes immediately
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Your account has been upgraded to Owner!";
            return RedirectToAction("Setup", "Property", new { area = "Admin" });
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

    }
}

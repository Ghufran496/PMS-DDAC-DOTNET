using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using PMS_DDAC.Models.ViewModels;
using PMS_DDAC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace PMS_DDAC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Owner")]
    [Area("Admin")]
    public class PropertyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly AzureEmailSender _emailSender;

        public PropertyController(AppDbContext context, UserManager<UserModel> userManager, AzureEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Setup()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var properties = _context.Properties.Where(p => p.OwnerId == user.Id).ToList();

            // 🔹 Fetch room counts for "Room" rental mode properties
            var roomCounts = _context.Rooms
                .Where(r => properties.Select(p => p.PropertyId).Contains(r.PropertyId))
                .GroupBy(r => r.PropertyId)
                .ToDictionary(g => g.Key, g => g.Count());

            var viewModel = new PropertySetupViewModel
            {
                ExistingProperties = properties,
                PropertyRoomCounts = roomCounts
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProperty(PropertySetupViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingProperties = _context.Properties.Where(p => p.OwnerId == user.Id).ToList();
            if (existingProperties.Count >= 2)
            {
                TempData["Error"] = "You can only create up to 2 properties.";
                return RedirectToAction("Setup");
            }

            var property = new PropertyModel
            {
                Name = model.Name,
                Address = model.Address,
                RentalMode = model.RentalMode,
                OwnerId = user.Id
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            if (model.RentalMode == "Room" && model.RoomNames != null)
            {
                foreach (var roomName in model.RoomNames)
                {
                    var room = new RoomModel
                    {
                        PropertyId = property.PropertyId,
                        RoomName = roomName
                    };
                    _context.Rooms.Add(room);
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Property setup completed!";
            return RedirectToAction("Setup");
        }
    }
}

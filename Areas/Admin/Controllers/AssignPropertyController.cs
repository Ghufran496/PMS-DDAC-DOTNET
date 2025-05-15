using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using PMS_DDAC.Models.ViewModels;
using PMS_DDAC.Services;

namespace PMS_DDAC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Owner")]
    [Area("Admin")]
    public class AssignPropertyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly AzureEmailSender _emailSender;

        public AssignPropertyController(AppDbContext context, UserManager<UserModel> userManager, AzureEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        
        [HttpGet]
        public async Task<IActionResult> AssignProperty()
        {
            var properties = await _context.Properties
                .Where(p =>
                    (p.RentalMode == "Unit" && p.TenantId == null) || // Unassigned units
                    (p.RentalMode == "Room" && _context.Rooms.Any(r => r.PropertyId == p.PropertyId && r.TenantId == null)) // Properties with vacant rooms
                )
                .Select(p => new SelectListItem { Value = p.PropertyId.ToString(), Text = p.Name })
                .ToListAsync();

            var tenants = await (from user in _userManager.Users
                                 join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where role.Name == "Tenant"
                                 select new SelectListItem
                                 {
                                     Value = user.Id,
                                     Text = user.FullName
                                 }).ToListAsync();

            var assignedTenantIds = await _context.Properties
                .Where(p => p.TenantId != null)
                .Select(p => p.TenantId)
                .ToListAsync();

            var assignedRoomTenantIds = await _context.Rooms
                .Where(r => r.TenantId != null)
                .Select(r => r.TenantId)
                .ToListAsync();

            assignedTenantIds.AddRange(assignedRoomTenantIds); 

            var filteredTenants = tenants
                .Where(t => !assignedTenantIds.Contains(t.Value))
                .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
                .ToList();



            var model = new AssignPropertyViewModel
            {
                Properties = properties,
                Tenants = filteredTenants,
                Rooms = new List<SelectListItem>()
            };

            return View("~/Areas/Admin/Views/Property/AssignProperty.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignProperty(AssignPropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns if model state is invalid
                model.Tenants = await (from user in _userManager.Users
                                       join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                       join role in _context.Roles on userRole.RoleId equals role.Id
                                       where role.Name == "Tenant"
                                       select new SelectListItem
                                       {
                                           Value = user.Id,
                                           Text = user.FullName
                                       }).ToListAsync();

                model.Properties = await _context.Properties
                    .Select(p => new SelectListItem
                    {
                        Value = p.PropertyId.ToString(),
                        Text = p.Name
                    }).ToListAsync();

                model.Rooms = new List<SelectListItem>(); // Ensure Rooms list is initialized

                var tenant = await _userManager.FindByIdAsync(model.SelectedTenantId);
                if (tenant == null)
                {
                    ModelState.AddModelError("", "Tenant not found.");
                    return await ReloadViewWithDropdowns(model);
                }

                var property = await _context.Properties.FindAsync(model.SelectedPropertyId);
                if (property == null)
                {
                    ModelState.AddModelError("", "Property not found.");
                    return await ReloadViewWithDropdowns(model);
                }

                if (property.RentalMode == "Room" && model.SelectedRoomId != null)
                {
                    // Assign the room to the tenant
                    var room = await _context.Rooms.FindAsync(model.SelectedRoomId);
                    if (room == null)
                    {
                        ModelState.AddModelError("", "Room not found.");
                        return await ReloadViewWithDropdowns(model);
                    }

                    room.TenantId = tenant.Id;
                    _context.Rooms.Update(room);
                }
                else
                {
                    // Assign the unit property to the tenant
                    property.TenantId = tenant.Id;
                    _context.Properties.Update(property);
                }

                await _context.SaveChangesAsync();

                // Send confirmation email
                string subject = "Property Assignment Confirmation";
                string message = $@"
            Dear {tenant.FullName},<br/><br/>
            You have been assigned to <strong>{property.RentalMode} - {property.Name}</strong>.<br/><br/>
            Thank you!
            ";

                await _emailSender.SendEmailAsync(tenant.Email, subject, message);

                TempData["Success"] = "Tenant assigned successfully!";

                return RedirectToAction("AssignProperty");

            }
            return RedirectToAction("AssignProperty");
        }

        // Helper method to reload dropdowns if validation fails
        private async Task<IActionResult> ReloadViewWithDropdowns(AssignPropertyViewModel model)
        {
            model.Tenants = await (from user in _userManager.Users
                                   join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                   join role in _context.Roles on userRole.RoleId equals role.Id
                                   where role.Name == "Tenant"
                                   select new SelectListItem
                                   {
                                       Value = user.Id,
                                       Text = user.FullName
                                   }).ToListAsync();

            model.Properties = await _context.Properties
                .Select(p => new SelectListItem
                {
                    Value = p.PropertyId.ToString(),
                    Text = p.Name
                }).ToListAsync();

            model.Rooms = new List<SelectListItem>(); // Ensure Rooms list is initialized

            return View("~/Areas/Admin/Views/Property/AssignProperty.cshtml", model);
        }


        [HttpGet]
        public async Task<IActionResult> GetRooms(int propertyId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.PropertyId == propertyId && r.TenantId == null)
                .Select(r => new SelectListItem
                {
                    Value = r.RoomId.ToString(),
                    Text = r.RoomName
                })
                .ToListAsync();

            return Json(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties(string rentalMode)
        {
            if (string.IsNullOrEmpty(rentalMode))
            {
                return Json(new List<SelectListItem>()); // Return empty list if no rental mode is provided
            }

            var properties = await _context.Properties
                .Where(p => p.RentalMode == rentalMode && p.TenantId ==null)
                .Select(p => new SelectListItem
                {
                    Value = p.PropertyId.ToString(),
                    Text = p.Name
                }).ToListAsync();

            return Json(properties);
        }
    }
}

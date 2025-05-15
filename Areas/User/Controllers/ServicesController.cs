using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using System.Threading.Tasks;

namespace PMS_DDAC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: User/Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .Include(s => s.Property)
                .Where(s => s.TenantId == User.Identity.Name && (s.ScheduledDate == null || s.ScheduledDate >= DateTime.UtcNow) && s.Status != "Completed")
                .ToListAsync();
            return View(services);
        }

        // GET: User/Services/Book
        public async Task<IActionResult> Book()
        {
            var properties = await _context.Properties
                .Where(p => p.TenantId == User.Identity.Name || p.OwnerId == User.Identity.Name)
                .ToListAsync();
            ViewBag.Properties = properties;

            // Initialize ServiceModel with required TenantId
            var service = new ServiceModel
            {
                TenantId = User.Identity.Name // Set the required TenantId
            };
            return View(service);
        }

        // POST: User/Services/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                service.TenantId = User.Identity.Name; // Ensure TenantId is set
                service.RequestedDate = DateTime.UtcNow;
                service.Status = "Requested";
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var properties = await _context.Properties
                .Where(p => p.TenantId == User.Identity.Name || p.OwnerId == User.Identity.Name)
                .ToListAsync();
            ViewBag.Properties = properties;
            return View(service);
        }

        // GET: User/Services/History
        public async Task<IActionResult> History()
        {
            var history = await _context.Services
                .Include(s => s.Property)
                .Where(s => s.TenantId == User.Identity.Name && (s.CompletedDate != null || s.Status == "Completed"))
                .ToListAsync();
            return View(history);
        }
    }
}
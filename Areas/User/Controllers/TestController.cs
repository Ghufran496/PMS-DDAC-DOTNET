using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PMS_DDAC.Area.User.Controllers
{
    [Area("User")]
    public class TestController : Controller
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Test/Services
        public async Task<IActionResult> Services()
        {
            var tenantId = "tenant1"; // Matches your dummy data
            var services = await _context.Services
                .Where(s => s.TenantId == tenantId)
                .Include(s => s.Property)
                .OrderByDescending(s => s.RequestedDate)
                .ToListAsync();

            ViewData["Title"] = "My Services"; // Matches your layout's title
            return View("~/Views/Test/Index.cshtml", services);
        }
    }
}
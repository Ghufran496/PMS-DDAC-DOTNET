using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PMS_DDAC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class BillsController : Controller
    {
        private readonly AppDbContext _context; // Updated to AppDbContext
        private readonly UserManager<UserModel> _userManager;

        public BillsController(AppDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User/Bills
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var bills = await _context.Bills
                .Include(b => b.Property)
                .Where(b => b.TenantId == user.Id)
                .ToListAsync();

            return View(bills);
        }

        // GET: User/Bills/Pay/5
        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var bill = await _context.Bills
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.BillId == id && b.TenantId == user.Id);

            if (bill == null)
            {
                return NotFound("Bill not found or you do not have access to this bill.");
            }

            if (bill.Status == "Paid")
            {
                TempData["Error"] = "This bill has already been paid.";
                return RedirectToAction(nameof(Index));
            }

            return View(bill);
        }

        // POST: User/Bills/Pay/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.BillId == id && b.TenantId == user.Id);

            if (bill == null)
            {
                return NotFound("Bill not found or you do not have access to this bill.");
            }

            if (bill.Status == "Paid")
            {
                TempData["Error"] = "This bill has already been paid.";
                return RedirectToAction(nameof(Index));
            }

            bill.Status = "Paid";
            bill.PaidDate = DateTime.UtcNow;

            _context.Update(bill);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment successful!";
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Bills/PaymentHistory
        public async Task<IActionResult> PaymentHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var paidBills = await _context.Bills
                .Include(b => b.Property)
                .Where(b => b.TenantId == user.Id && b.Status == "Paid")
                .OrderByDescending(b => b.PaidDate)
                .ToListAsync();

            return View(paidBills);
        }
    }
}
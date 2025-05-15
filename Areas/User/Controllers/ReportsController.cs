using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS_DDAC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public ReportsController(AppDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User/Reports
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: User/Reports/Electricity
        public async Task<IActionResult> Electricity(int? year)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Default to current year if not specified
            int selectedYear = year ?? DateTime.UtcNow.Year;

            // Fetch bills for the tenant, assuming electricity bills have "Electricity" in the property name
            var bills = await _context.Bills
                .Include(b => b.Property)
                .Where(b => b.TenantId == user.Id && b.Property.Name.Contains("Electricity"))
                .ToListAsync();

            // Group bills by month and year
            var monthlyReports = bills
                .GroupBy(b => new { b.PaidDate?.Year, b.PaidDate?.Month })
                .Where(g => g.Key.Year == selectedYear && g.Key.Month != null) // Filter by selected year
                .Select(g => new MonthlyBillReportViewModel
                {
                    Year = g.Key.Year ?? selectedYear,
                    Month = g.Key.Month ?? 1,
                    TotalAmount = g.Sum(b => b.Amount),
                    Bills = g.ToList()
                })
                .OrderBy(r => r.Month)
                .ToList();

            ViewBag.SelectedYear = selectedYear;
            ViewBag.Years = Enumerable.Range(2020, DateTime.UtcNow.Year - 2019).ToList(); // Years from 2020 to current year

            return View(monthlyReports);
        }

        // GET: User/Reports/Water
        public async Task<IActionResult> Water(int? year)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Default to current year if not specified
            int selectedYear = year ?? DateTime.UtcNow.Year;

            // Fetch bills for the tenant, assuming water bills have "Water" in the property name
            var bills = await _context.Bills
                .Include(b => b.Property)
                .Where(b => b.TenantId == user.Id && b.Property.Name.Contains("Water"))
                .ToListAsync();

            // Group bills by month and year
            var monthlyReports = bills
                .GroupBy(b => new { b.PaidDate?.Year, b.PaidDate?.Month })
                .Where(g => g.Key.Year == selectedYear && g.Key.Month != null) // Filter by selected year
                .Select(g => new MonthlyBillReportViewModel
                {
                    Year = g.Key.Year ?? selectedYear,
                    Month = g.Key.Month ?? 1,
                    TotalAmount = g.Sum(b => b.Amount),
                    Bills = g.ToList()
                })
                .OrderBy(r => r.Month)
                .ToList();

            ViewBag.SelectedYear = selectedYear;
            ViewBag.Years = Enumerable.Range(2020, DateTime.UtcNow.Year - 2019).ToList(); // Years from 2020 to current year

            return View(monthlyReports);
        }
    }
}
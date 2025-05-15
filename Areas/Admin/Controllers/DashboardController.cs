using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Data;
using PMS_DDAC.Models.Admin; 
using PMS_DDAC.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace PMS_DDAC.Areas.Admin.Controllers
{
    [Area("Admin")] // Ensure the area is set
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 🔹 Count number of bills marked as paid
            var paidBillsCount = _context.Bills.Count(b => b.Status == "Paid");
            var unpaidBillsCount = _context.Bills.Count(b => b.Status == "Unpaid");

            // 🔹 Calculate total paid & unpaid income
            var paidIncome = _context.Bills.Where(b => b.Status == "Paid").Sum(b => b.Amount);
            var unpaidIncome = _context.Bills.Where(b => b.Status == "Unpaid").Sum(b => b.Amount);

            decimal totalUtilityBills = _context.Bills.ToList().Sum(b => b.Amount);
            
            decimal totalWaterBill = _context.Bills
                .Where(b => b.Property.Name.Contains("Water") )// Ensure `Type` exists in your `Bill` model
                .Sum(b => b.Amount);

            decimal totalElectricityBill = _context.Bills
                .Where(b => b.Property.Name.Contains ( "Electricity")) // Ensure `Type` exists in your `Bill` model
                .Sum(b => b.Amount);



            // 🔹 Get upcoming scheduled services (limit to 2)
            var upcomingServices = _context.Services
                .OrderBy(s => s.ScheduledDate)
                .Take(2)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                PaidBillsCount = paidBillsCount,
                UnpaidBillsCount = unpaidBillsCount,
                PaidIncome = paidIncome,
                UnpaidIncome = unpaidIncome,
                TotalWaterBill = totalWaterBill,
                TotalElectricityBill = totalElectricityBill,
                UpcomingServices = upcomingServices
            };

            return View(model);
        }
    }
}

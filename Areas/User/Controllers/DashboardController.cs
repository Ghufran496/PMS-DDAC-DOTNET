using Microsoft.AspNetCore.Mvc;

namespace PMS_DDAC.Area.User.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

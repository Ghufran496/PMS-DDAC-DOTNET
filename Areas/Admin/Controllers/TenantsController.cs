using Microsoft.AspNetCore.Mvc;

namespace PMS_DDAC.Area.Admin.Controllers
{
    public class TenantsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

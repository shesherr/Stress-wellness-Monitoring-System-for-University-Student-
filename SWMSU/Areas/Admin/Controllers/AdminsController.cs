using Microsoft.AspNetCore.Mvc;

namespace SWMSU.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminsController : Controller
    {
        public IActionResult Admindashboard()
        {
            return View();
        }
    }
}

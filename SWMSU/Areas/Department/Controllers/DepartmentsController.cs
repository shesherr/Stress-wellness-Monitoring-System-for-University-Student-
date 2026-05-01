using Microsoft.AspNetCore.Mvc;

namespace SWMSU.Areas.Department.Controllers
{
    [Area("Department")]
    public class AdminsController : Controller
    {
        public IActionResult Departmentdashboard()
        {
            return View();
        }
    }
}

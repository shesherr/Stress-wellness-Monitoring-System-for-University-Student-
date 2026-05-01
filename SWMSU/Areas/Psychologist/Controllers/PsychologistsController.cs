using Microsoft.AspNetCore.Mvc;

namespace SWMSU.Areas.Psychologist.Controllers
{
    [Area("Psychologist")]
    public class PsychologistsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PsychologistDashboard()
        {
            
            return View();
        }

    }
}

using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Psychologist.Controllers
{
    [Area("Psychologist")]
    public class SeeTheStudentListController : Controller
    {
        private readonly DapperContext _context;

        public SeeTheStudentListController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult StudentList()
        {
            using var connection = _context.CreateConnection();

            var psychiatristIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(psychiatristIdString))
                return RedirectToAction("Login", "Account");

            int psychiatristId = Convert.ToInt32(psychiatristIdString);

            var param = new DynamicParameters();
            param.Add("@flag", 18);
            param.Add("@PsychiatristId", psychiatristId);

            var model = connection
                .Query<StudentAppointmentVM>(
                    "Sp_BookAppointment",
                    param,
                    commandType: CommandType.StoredProcedure
                )
                .ToList();

            return View(model);
        }
    

    }
}

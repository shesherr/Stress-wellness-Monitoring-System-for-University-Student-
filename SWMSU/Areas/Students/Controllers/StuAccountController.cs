using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;     // Student model namespace
using System.Data;
namespace SWMSU.Areas.Students.Controllers
{
    [Area("Students")] 
    public class StuAccountController : Controller
    {
        private readonly DapperContext _context;
        public StuAccountController(DapperContext context)
        {
            _context = context;
        }
        public IActionResult StuRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult StuRegistration( Student s )
        {

            using var con = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@StudentVarsityId", s.StudentVarsityId);
            param.Add("@StudentName", s.StudentName);
            param.Add("@CurrentSemester", s.CurrentSemester);
            param.Add("@StudentEmail", s.StudentEmail);
            param.Add("@StudentPassword", s.StudentPassword);
            param.Add("@CreditCurrent", s.CreditCurrent);
            param.Add("@StudentPermanentAddress", s.StudentPermanentAddress);
            param.Add("@StudentBloodGroup", s.StudentBloodGroup);
            param.Add("@StudentPhone", s.StudentBloodGroup);
            param.Add("@Role", s.Role);
            con.Execute("SP_Student", param, commandType: CommandType.StoredProcedure);

            TempData["success"] = "Registration Successful!";
            return RedirectToAction("StuLogin");
        }
        // =================== LOGIN (GET) ===================
        [HttpGet]
        public IActionResult StuLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult StuLogin(Student s)
        {
            var query = "SELECT * FROM Students WHERE StudentEmail = @Email AND StudentPassword = @Password";

            using (var connection = _context.CreateConnection())
            {
                var student = connection.QueryFirstOrDefault<Student>(query,
     new { Email = s.StudentEmail, Password = s.StudentPassword });

                if (student == null)
                {
                    TempData["error"] = "Invalid Email or Password!";
                    return View();
                }

                // Store Session
                HttpContext.Session.SetInt32("StudentId", student.StudentId);
                HttpContext.Session.SetString("StudentName", student.StudentName);
                HttpContext.Session.SetString("Role", student.Role);

                return RedirectToAction("StudentDashboard", "Student", new { area = "Students" });
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // সব Session remove
            return RedirectToAction("StuLogin");
        }
    }

}

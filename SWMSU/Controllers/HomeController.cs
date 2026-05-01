using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SWMSU.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DapperContext _context;
        public HomeController(ILogger<HomeController> logger, DapperContext context)
        {
            _logger = logger;
            _context = context; // ← This fixes the null reference
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();

        }
        public IActionResult Student()
        {
            return View();

        }
        public IActionResult Teacher()
        {
            return View();

        }
        public IActionResult Form()
        {
            return View();

        }
        public IActionResult Registration()
        {
            using var con = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@flag", 2);   // Fetch user types

            var userTypes = con.Query<UserType>(
                "SP_User",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            ViewBag.UserTypeList = new SelectList(userTypes, "UserTypeId", "UserTypeName");

            return View(new UserVM());
        }
        [HttpPost]
        public IActionResult Registration(UserVM u)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all required fields.";
                return View(u);
            }

            using var con = _context.CreateConnection();
            var param = new DynamicParameters();

            param.Add("@UserName", u.UserName ?? "");
            param.Add("@Email", u.Email ?? "");
            param.Add("@Password", u.Password ?? "");
            param.Add("@PhoneNumber", u.PhoneNumber ?? "");
            param.Add("@Age", u.Age);
            param.Add("@Address", u.Address ?? "");
            param.Add("@BloodGroup", u.BloodGroup ?? "");
            param.Add("@UsertypeId", u.UsertypeId);
            param.Add("@Status", u.Status ?? "Pending");
            param.Add("@flag", 1);  // Insert

            con.Execute("SP_User", param, commandType: CommandType.StoredProcedure);

            TempData["success"] = "Registration Successful!";
            return RedirectToAction("Login", "Home");
        }


        public IActionResult Login()
        {
            return View();

        }

        //[HttpPost]
        //public IActionResult Login(UserVM model)
        //{
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@flag", 3); // login flag
        //        parameters.Add("@Email", model.Email);
        //        parameters.Add("@Password", model.Password);

        //        var data = connection.QueryFirstOrDefault<UserVM>(
        //            "SP_User",
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //        );

        //        if (data == null)
        //        {
        //            TempData["error"] = "Invalid Entry";
        //            return RedirectToAction("Login"); // redirect back to login
        //        }

        //        // Optional: check approval status
        //        if ((data.UsertypeId == 2 || data.UsertypeId == 3) && data.Status != "Approved")
        //        {
        //            TempData["error"] = "Your account is not approved yet.";
        //            return RedirectToAction("Login");
        //        }

        //        // Set session
        //        HttpContext.Session.SetString("UserEmail", data.Email);
        //        HttpContext.Session.SetString("UserName", data.UserName);
        //        HttpContext.Session.SetString("UserId", data.UserId.ToString());
        //        HttpContext.Session.SetString("UsertypeId", data.UsertypeId.ToString());

        //        // Redirect to dashboard based on user type
        //        if (data.UsertypeId == 1)
        //        {
        //            return RedirectToAction("StudentDashboard", "Student", new { area = "Students" });
        //        }
        //        else if (data.UsertypeId == 2)
        //        {
        //            return RedirectToAction("PsychologistDashboard", "Psychologists", new { area = "Psychologist" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("Admindashboard", "Admins", new { area = "Admin" });
        //        }
        //    }
        //}
        [HttpPost]
        public IActionResult Login(UserVM model)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@flag", 3); // login
            parameters.Add("@Email", model.Email);
            parameters.Add("@Password", model.Password);

            var data = connection.QueryFirstOrDefault<UserVM>(
                "SP_User",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // ❌ User not found
            if (data == null)
            {
                TempData["error"] = "Invalid Email or Password";
                return RedirectToAction("Login");
            }

            // ❌ Not approved
            if (data.Status != "Approved")
            {
                TempData["error"] = "Your account is not approved yet.";
                return RedirectToAction("Login");
            }

            // ✅ Set session
            HttpContext.Session.SetString("UserEmail", data.Email);
            HttpContext.Session.SetString("UserName", data.UserName);
            HttpContext.Session.SetString("UserId", data.UserId.ToString());
            HttpContext.Session.SetString("UsertypeId", data.UsertypeId.ToString());

            // ✅ Redirect by UserType
            return data.UsertypeId switch
            {
                1 => RedirectToAction("StudentDashboard", "Student", new { area = "Students" }),
                2 => RedirectToAction("PsychologistDashboard", "Psychologists", new { area = "Psychologist" }),
                3 => RedirectToAction("Admindashboard", "Admins", new { area = "Admin" }),
                _ => RedirectToAction("Index", "Home")
            };
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync();

            // Clear session completely
            HttpContext.Session.Clear();

            // Redirect to Homepage
            return RedirectToAction("Index", "Home");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

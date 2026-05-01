using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PsychologistStudentController : Controller
    {
        private readonly DapperContext _context;
        public PsychologistStudentController(DapperContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PsychologistList()
        {
            using (var connection = _context.CreateConnection())
            {

                var p = new DynamicParameters();
                p.Add("@flag", 8);
                var data = connection.Query<UserVM>(
                    "Sp_User",
                    p,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }
        public IActionResult StudentList()
        {
            using (var connection = _context.CreateConnection())
            {

                var p = new DynamicParameters();
                p.Add("@flag", 9);
                var data = connection.Query<UserVM>(
                    "Sp_User",
                    p,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }
        public IActionResult Edit(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var p = new DynamicParameters();
                p.Add("@flag", 10); // Fetch single user
                p.Add("@UserId", id); // Correct parameter name
                var data = connection.QueryFirstOrDefault<UserVM>(
                    "Sp_User",
                    p,
                    commandType: CommandType.StoredProcedure
                );
                return View(data);
            }
        }
        [HttpPost]

        public IActionResult Edit(UserVM model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@flag", 14);
                    p.Add("@UserId", model.UserId);
                    p.Add("@UserName", model.UserName);
                    p.Add("@Email", model.Email);
                    p.Add("@PhoneNumber", model.PhoneNumber);

                    p.Add("@Age", model.Age);
                    p.Add("@Address", model.Address);
                    p.Add("@BloodGroup", model.BloodGroup);

                    connection.Execute(
                        "Sp_User",
                        p,
                        commandType: CommandType.StoredProcedure
                    );
                }

                TempData["Message"] = "User updated successfully!";
                if (model.UsertypeId == 2)
                {
                    return RedirectToAction("PsychologistList");
                }
                return RedirectToAction("StudentList");
            }
            catch (Exception ex)
            {
                // Optional: log the error
                TempData["Error"] = "An error occurred while updating the user: " + ex.Message;
                return View(model);
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                int userTypeId;
                using (var connection = _context.CreateConnection())
                {
                    // First fetch the user type
                    userTypeId = connection.QueryFirstOrDefault<int>(
                        "SELECT UsertypeId FROM Users WHERE UserId = @UserId",
                        new { UserId = id }
                    );

                    // Now delete the user
                    var parameters = new DynamicParameters();
                    parameters.Add("@flag", 11); // Delete
                    parameters.Add("@UserId", id);

                    connection.Execute(
                        "Sp_User",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                TempData["Message"] = "User deleted successfully!";

                if (userTypeId == 2)
                {
                    return RedirectToAction("PsychologistList");
                }
                return RedirectToAction("StudentList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the user.";
                return RedirectToAction("PatientInfo");
            }
        }

        public IActionResult Approve(int id)
        {
            try
            {
                int userTypeId;
                using (var connection = _context.CreateConnection())
                {
                    // Get the user type first (same idea as Delete)
                    userTypeId = connection.QueryFirstOrDefault<int>(
                        "SELECT UsertypeId FROM Users WHERE UserId = @UserId",
                        new { UserId = id }
                    );

                    // Now approve the user
                    var parameters = new DynamicParameters();
                    parameters.Add("@flag", 12); // Approve
                    parameters.Add("@UserId", id);

                    connection.Execute(
                        "Sp_User",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                TempData["Message"] = "User approved successfully!";

                if (userTypeId == 2)
                {
                    return RedirectToAction("PsychologistList");
                }
                return RedirectToAction("StudentList");
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while approving the user.";
                return RedirectToAction("PatientInfo");
            }
        }







    }

}
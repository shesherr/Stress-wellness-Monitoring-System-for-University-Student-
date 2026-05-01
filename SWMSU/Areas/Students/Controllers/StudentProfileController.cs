using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Students.Controllers
{
    [Area("Students")]
    public class StudentProfileController : Controller
    {
        private readonly DapperContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentProfileController(DapperContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult ViewProfile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("StuLogin");
            }

            int userId = Convert.ToInt32(userIdString);

            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@flag", 2);
                parameters.Add("@UserId", userId);

                var profile = connection.QueryFirstOrDefault<StudentProfileVM>(
                    "SP_StudentProfile",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (profile == null)
                {
                    TempData["Error"] = "Profile not found.";
                    return RedirectToAction("StuProfile"); // বা অন্য suitable page
                }

                return View(profile);
            }
        }


        public IActionResult StuProfile()
        {
            return View();
        }

        private string UploadedFile(StudentProfileVM model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Student");
                Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.ImageFile.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

        [HttpPost]
        public IActionResult StuProfile(StudentProfileVM p)
        {
            string imageName = UploadedFile(p);

            using var con = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@UserId", Convert.ToInt32(HttpContext.Session.GetString("UserId")));
            param.Add("@StudentProfileId", p.StudentProfileId);
            param.Add("@StudentId", p.StudentId);
            param.Add("@Semester", p.Semester);
            param.Add("@CGPA", p.CGPA);
            param.Add("@Imageurl", imageName);
            param.Add("@Department", p.Department);
            param.Add("@Credit", p.Credit);
            param.Add("@flag", 1); // INSERT

            con.Execute("SP_StudentProfile", param, commandType: CommandType.StoredProcedure);

            TempData["success"] = "Profile saved successfully!";
            return RedirectToAction("ViewProfile");
        }
       
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            using var con = _context.CreateConnection();

            var param = new DynamicParameters();
            param.Add("@flag", 3); // 🔥 NEW FLAG
            param.Add("@StudentProfileId", id);

            var profile = con.QueryFirstOrDefault<StudentProfileVM>(
                "SP_StudentProfile",
                param,
                commandType: CommandType.StoredProcedure
            );

            if (profile == null)
                return NotFound();

            return View(profile);
        }


        
        [HttpPost]
        public IActionResult EditProfile(StudentProfileVM p)
        {
            using var con = _context.CreateConnection();

            string imageName;

            // 🔥 Case 1: New image uploaded
            if (p.ImageFile != null && p.ImageFile.Length > 0)
            {
                imageName = UploadedFile(p);
            }
            else
            {
                // 🔥 Case 2: No new image → get old image from DB
                var imgParam = new DynamicParameters();
                imgParam.Add("@flag", 3);
                imgParam.Add("@StudentProfileId", p.StudentProfileId);

                imageName = con.QuerySingle<string>(
                    "SELECT Imageurl FROM StudentProfile WHERE StudentProfileId = @StudentProfileId",
                    new { p.StudentProfileId }
                );
            }

            var param = new DynamicParameters();
            param.Add("@flag", 4);
            param.Add("@UserId", p.UserId);

            param.Add("@UserName", p.UserName);
            param.Add("@Email", p.Email);
            param.Add("@Address", p.Address);
            param.Add("@BloodGroup", p.BloodGroup);

            param.Add("@StudentId", p.StudentId);
            param.Add("@Semester", p.Semester);
            param.Add("@CGPA", p.CGPA);
            param.Add("@Credit", p.Credit);
            param.Add("@Department", p.Department);
            param.Add("@Imageurl", imageName);

            con.Execute("SP_StudentProfile", param, commandType: CommandType.StoredProcedure);

            return RedirectToAction("ViewProfile");
        }
        //GetPsychology

        // ================== File Upload Helper ==================

    }
}

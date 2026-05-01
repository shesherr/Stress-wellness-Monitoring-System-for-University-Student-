using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Psychologist.Controllers
{
    [Area("Psychologist")]
    public class PsyProfileController : Controller
    {
        private readonly DapperContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PsyProfileController(
            DapperContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ============================
        // GET: CREATE / EDIT PROFILE
        // ============================
        public IActionResult PsychologyProfile()
        {
            return View();
        }

        // ============================
        // GET: VIEW PROFILE
        // ============================
        public IActionResult PsychoViewProfile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("PsychologyProfile");
            }

            int userId = Convert.ToInt32(userIdString);

            using var connection = _context.CreateConnection();

            var profile = connection.QueryFirstOrDefault<PsychologyProfile>(
                "SP_PsychologyProfile",
                new
                {
                    Flag = 2,
                    UserId = userId
                },
                commandType: CommandType.StoredProcedure
            );

            if (profile == null)
            {
                TempData["Error"] = "Profile not found.";
                return RedirectToAction("PsychologyProfile");
            }

            return View(profile);
        }

        // ============================
        // POST: SAVE PROFILE
        // ============================
        [HttpPost]
        public IActionResult PsychologyProfile(PsychologyProfile p)
        {
            // 🔥 Save image and get filename
            string imageName = UploadedFile(p);

            using var con = _context.CreateConnection();

            var param = new DynamicParameters();
            param.Add("@Flag", 1);
            param.Add("@UserId", Convert.ToInt32(HttpContext.Session.GetString("UserId")));

            param.Add("@Room", p.Room);
            param.Add("@Bio", p.Bio);
            param.Add("@ExperienceYears", p.ExperienceYears);
            param.Add("@Specialization", p.Specialization);

            // 🔥 THIS IS THE MAIN FIX
            param.Add("@ImageUrl", imageName);

            param.Add("@Qualification", p.Qualification);
            param.Add("@SeverityType", p.SeverityType);

            con.Execute(
                "SP_PsychologyProfile",
                param,
                commandType: CommandType.StoredProcedure
            );

            TempData["success"] = "Profile saved successfully!";
            return RedirectToAction("PsychoViewProfile");
        }

       

        private string UploadedFile(PsychologyProfile model)
        {
            if (model.ImageFile == null || model.ImageFile.Length == 0)
                return null;

            string uploadsFolder =
                Path.Combine(_webHostEnvironment.WebRootPath, "Images/Psychology");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName =
                Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ImageFile.CopyTo(fileStream);

            return uniqueFileName;
        }
    }
}

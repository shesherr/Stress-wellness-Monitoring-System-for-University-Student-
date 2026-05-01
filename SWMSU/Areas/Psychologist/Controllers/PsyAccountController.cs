using Dapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models; // আপনার Psychologist model namespace
using System.Data;

namespace SWMSU.Areas.Psychologist.Controllers
{
    [Area("Psychologist")]
    public class PsyAccountController : Controller
    {
        private readonly DapperContext _context;
        public PsyAccountController(DapperContext context)
        {
            _context = context;
        }
      

        // GET: Registration form
        public IActionResult PsyRegistration()
        {
            return View();
        }

        // POST: Handle registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PsyRegistration(Psychologists p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            // Dapper parameter mapping
            using var con = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@PsychoName", p.PsychoName);
            param.Add("@PsychoEmail", p.PsychoEmail);
            param.Add("@PsychoPassword", p.PsychoPassword);
            param.Add("@PsychoPhone", p.PsychoPhone ?? string.Empty);
            param.Add("@Qualification", p.Qualification ?? string.Empty);
            param.Add("@Specialization", p.Specialization ?? string.Empty);
            param.Add("@Experience", p.Experience);
            param.Add("@Role", p.Role );
            param.Add("@PsychoPermanentAddress", p.PsychoPermanentAddress ?? string.Empty);

            // Execute stored procedure
           
            con.Execute("SP_Psychologist", param, commandType: CommandType.StoredProcedure);
            TempData["SuccessMessage"] = "Registration successful! Psycho ID: ";
            return RedirectToAction("PsyRegistration");
        }
    }
}

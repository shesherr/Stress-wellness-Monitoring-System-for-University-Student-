//studentcontroller

using Dapper;          // <-- Add this
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Students.Controllers
{
    [Area("Students")]
    public class StudentController : Controller
    {
        private readonly DapperContext _context;
        public StudentController(DapperContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentDashboard()
        {
           
            return View();
        }

        public IActionResult Form()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Form(Dass21Form model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Auto calculate Total
            int D = model.Q2 + model.Q3 +
                           model.Q5 +
                          model.Q6 + model.Q8 + model.Q9 + model.Q14;
            model.Depression = D * 2;
            int S = model.Q1 + model.Q4 + model.Q5 +
                           model.Q7 + model.Q8 + model.Q10 +
                          model.Q13;
            model.Stress = S * 2;


            using var con = _context.CreateConnection();
            var param = new DynamicParameters();


            param.Add("@Date", model.Date);
            param.Add("@Q1", model.Q1);
            param.Add("@Q2", model.Q2);
            param.Add("@Q3", model.Q3);
            param.Add("@Q4", model.Q4);
            param.Add("@Q5", model.Q5);
            param.Add("@Q6", model.Q6);
            param.Add("@Q7", model.Q7);
            param.Add("@Q8", model.Q8);
            param.Add("@Q9", model.Q9);
            param.Add("@Q10", model.Q10);
            param.Add("@Q11", model.Q11);
            param.Add("@Q12", model.Q12);
            param.Add("@Q13", model.Q13);
            param.Add("@Q14", model.Q14);
            param.Add("@UserId", Convert.ToInt32(HttpContext.Session.GetString("UserId")));
            param.Add("@Depression", model.Depression);
            param.Add("@Stress", model.Stress);
            param.Add("@Flag", 1);

            con.Execute("Sp_Dash21Form", param, commandType: CommandType.StoredProcedure);

            return RedirectToAction("Dass21Result");
        }
        public IActionResult Dass21Result()
        {
        
            using var con = _context.CreateConnection();

            // সব রেকর্ড নেওয়া
            var resultList = con.Query<Dass21FormResultVM>(
                "Sp_Dash21Form",
                new { Flag = 2 },   // 2 = SELECT
                commandType: CommandType.StoredProcedure
            ).ToList(); // List তে কনভার্ট
            foreach (var item in resultList)
            {
                item.SeverityType = CalculateSeverityType(
                    item.Depression,
                    item.Stress
                );
            }

            return View(resultList); // List পাঠানো হচ্ছে
        }
        private string CalculateSeverityType(int depression, int stress)
        {
            int maxScore = Math.Max(depression, stress);

            if (maxScore <= 9)
                return "Normal";
            else if (maxScore <= 20)
                return "Medium";
            else
                return "Severe";
        }
        public IActionResult GetPsychologistBySevere(string severity)
        {
            using var con = _context.CreateConnection();

            var psychologists = con.Query<PsychologyProfile>(
                "SP_PsychologyProfile",
                new
                {
                    Flag = 4,
                    UserId = (int?)null,
                    SeverityType = severity
                },
                commandType: CommandType.StoredProcedure
            ).ToList();

            ViewBag.Severity = severity;

            return View(psychologists);
        }
        [HttpGet]
        public IActionResult Dass21AnswerDetails(int formId)
        {
            if (formId <= 0)
                return BadRequest("Invalid FormId");
           
            using var con = _context.CreateConnection();

            var form = con.QueryFirstOrDefault<Dass21Form>(
                "dbo.Sp_Dash21Form",
                new
                {
                    Flag = 4,
                    FormId = formId
                },
                commandType: CommandType.StoredProcedure
            );

            if (form == null)
                return NotFound();

            return View(form);   // ✅ SINGLE object
        }

    }
}

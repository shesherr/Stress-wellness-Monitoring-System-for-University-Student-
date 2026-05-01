using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWMSU.Data;
using SWMSU.Interface;
using SWMSU.Models;
using System.Data;
namespace SWMSU.Areas.Students.Controllers
{
    [Area("Students")]
    public class PrescriptionController : Controller
    {




        private readonly DapperContext _context;


        public PrescriptionController(DapperContext context)
        {
            _context = context;

        }
        public IActionResult PrescriptionList()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@flag", 20);
            parameters.Add("@UserId", userId);

            var bookings = connection.Query<BookingHistoryVM>(
                "Sp_BookAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            return View(bookings);
        }
        public IActionResult ViewPrescription(int bookingId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@flag", 2);
            parameters.Add("@BookingId", bookingId);

            var prescriptions = connection.Query<PrescriptionVM>(
                "Sp_Prescription",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            return View(prescriptions); // cshtml model = IEnumerable<PrescriptionVM>
        }


    }
}

using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Interface;
using SWMSU.Models;
using SWMSU.Service;
using System.Data;

namespace SWMSU.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MentalissueResultController : Controller
    {
        private readonly DapperContext _context;
        private readonly IMail _mailService;
        public MentalissueResultController(DapperContext context, IMail mailService)
        {
            _context = context;
            _mailService = mailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewMentalissueList()
        {
            using (var connection = _context.CreateConnection())
            {

                var p = new DynamicParameters();
                p.Add("@flag", 1);
                var data = connection.Query<MentalissueListVM>(
                    "Sp_Mentalissue",
                    p,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }
        public IActionResult GetFullListOfStudent()
        {
            using (var connection = _context.CreateConnection())
            {
                var p = new DynamicParameters();
                p.Add("@flag", 4);
                var data = connection.Query<AdminGetAllStudent>(
                    "Sp_Mentalissue",
                    p,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
                return View(data);
            }
        }

        public IActionResult StudentMentalHistoryById(int userId)
        {
            using (var connection = _context.CreateConnection())
            {
                var p = new DynamicParameters();
                p.Add("@flag", 6);
                p.Add("@UserId", userId);  

                var data = connection.Query<StudentMentalHistoryVM>(
                    "Sp_GetBYMentalissue",
                    p,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendAbsentMessage(int BookingId)
        {
            using var connection = _context.CreateConnection();

            // Get booking details
            var parameters = new DynamicParameters();
            parameters.Add("@flag", 16);              // same SP flag
            parameters.Add("@BookingId", BookingId);

            var booking = connection.QueryFirstOrDefault<BookingHistoryVM>(
                "Sp_BookAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (booking == null || string.IsNullOrEmpty(booking.UserEmail))
            {
                TempData["Error"] = "Student email not found!";
                return RedirectToAction("AllStudentBookings");
            }

            try
            {
                string subject = $"Absent Notification (Booking ID: {booking.BookingId})";

                string body = $@"
Dear {booking.PatientName},<br/><br/>

This is to inform you that you were marked as 
<b style='color:red;'>ABSENT</b> for your scheduled psychology appointment.<br/><br/>
<b style='color:red;'>You missed this appointment.</b><br/><br/>

If this was a mistake or you need to reschedule, please contact the Psychology Department as soon as possible.<br/><br/>

Regards,<br/>
<b>Psychology Department</b><br/>
IUBAT
<b>Appointment Details:</b><br/>
Booking ID: {booking.BookingId}<br/>
Student Name: {booking.PatientName}<br/>
Psychologist: {booking.PsychiatristName}<br/>
Appointment Date: {booking.AppointmentDate:dd MMM yyyy}<br/>
Appointment Time: {booking.AppointmentTime}<br/>
Appointment Day: {booking.AppointmentDay}<br/><br/>


";

                await _mailService.SendEmailAsync(
                    booking.UserEmail,
                    subject,
                    body
                );

                TempData["Success"] = "Absent notification sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending mail: " + ex.Message;
            }

            return RedirectToAction("GetFullListOfStudent");
        }

    }
}


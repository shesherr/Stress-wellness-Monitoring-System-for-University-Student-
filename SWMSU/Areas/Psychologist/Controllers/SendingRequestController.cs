using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWMSU.Data;
using SWMSU.Interface;
using SWMSU.Models;
using System.Data;

namespace SWMSU.Areas.Psychologist.Controllers
{
    [Area("Psychologist")]
    public class SendingRequestController : Controller
    {
        private readonly DapperContext _context;
        private readonly IMail _mailService;
        public SendingRequestController(DapperContext context, IMail mailService)
        {
            _context = context;
            _mailService = mailService;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(int BookingId, string Message, string AppointmentTime)
        {
            using var connection = _context.CreateConnection();

            // Get booking details
            var parameters = new DynamicParameters();
            parameters.Add("@flag", 16);
            parameters.Add("@BookingId", BookingId);

            var booking = connection.QueryFirstOrDefault<BookingHistoryVM>(
                "Sp_BookAppointment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (booking == null || string.IsNullOrEmpty(booking.UserEmail))
            {
                TempData["Error"] = "Patient email not found!";
                return RedirectToAction("ViewRequest");
            }

            try
            {
                string subject = $"Message Regarding Your Appointment (Booking ID: {booking.BookingId})";
                string body = $@"
Dear {booking.PatientName},<br/><br/>
You have received a message from your psychiatrist <b>{booking.PsychiatristName}</b> regarding your appointment.<br/><br/>
<b>Booking Details:</b><br/>
Booking ID: {booking.BookingId}<br/>
Patient Name: {booking.PatientName}<br/>
Psychiatrist: {booking.PsychiatristName}<br/>
Appointment Date: {booking.AppointmentDate.ToShortDateString()}<br/>
Appointment Time: {AppointmentTime}<br/>  <!-- Use selected time here -->
Appointment Day: {booking.AppointmentDay}<br/>
BookingSerial: {booking.BookingSerial}<br/><br/>
<b>Message from Psychiatrist:</b><br/>
{Message}<br/><br/>
Regards,<br/>
{booking.PsychiatristName}
";

                await _mailService.SendEmailAsync(booking.UserEmail, subject, body);
                TempData["Success"] = "Message sent to patient successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending message: " + ex.Message;
            }

            return RedirectToAction("ViewRequest");
        }
        [HttpGet]
        public IActionResult ViewRequest()
        {
            // Session থেকে logged-in psychologist UserId নাও
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            int psychologistUserId = Convert.ToInt32(userIdString);

            using var connection = _context.CreateConnection();

            // Dapper দিয়ে SP call
            var data = connection.Query<CounsellingRequest>(
                "Sp_BookAppointment",
                new
                {
                    flag = 3,
                    UserId = psychologistUserId
                },
                commandType: CommandType.StoredProcedure
            ).ToList();

            // View-এ পাঠাও
            return View(data);
        }

        [HttpPost]
        public IActionResult Approve(int BookingId)
        {
            using var connection = _context.CreateConnection();

            connection.Execute(
                "Sp_BookAppointment",
                new
                {
                    Flag = 8,           // 🔴 MUST
                    BookingId = BookingId
                },
                commandType: CommandType.StoredProcedure
            );

            TempData["Success"] = "Request approved successfully!";
            return RedirectToAction("ViewRequest");
        }


        [HttpPost]
        public IActionResult Completed(int BookingId)
        {
            using var connection = _context.CreateConnection();

            connection.Execute(
                "Sp_BookAppointment",
                new
                {
                    Flag = 17,

                    BookingId = BookingId
                },
                commandType: CommandType.StoredProcedure
            );

            TempData["Success"] = "Appointment marked as completed!";
            return RedirectToAction("ViewRequest");
        }
        public IActionResult Absent(int BookingId)
        {
            using var connection = _context.CreateConnection();

            connection.Execute(
                "Sp_BookAppointment",
                new
                {
                    Flag = 19,

                    BookingId = BookingId
                },
                commandType: CommandType.StoredProcedure
            );

            TempData["Success"] = "Appointment marked as completed!";
            return RedirectToAction("ViewRequest");
        }
        //Prescriptiion
        public async Task<IActionResult> Prescription(int bookingId)
        {
            var medParams = new DynamicParameters();
            medParams.Add("@flag", 1);

            var medicine = await _context.CreateConnection().QueryAsync<dynamic>(
                "Sp_NewMedicine",
                medParams,
                commandType: CommandType.StoredProcedure
            );

            var presParams = new DynamicParameters();
            presParams.Add("@flag", 3);
            presParams.Add("@BookingId", bookingId);

            var prescriptionInfo = await _context.CreateConnection().QueryAsync<PrescriptionVM>(
                "Sp_Prescription",
                presParams,
                commandType: CommandType.StoredProcedure
            );

            ViewBag.Medicine = medicine;
            ViewBag.BookingId = bookingId;

            // Model হিসাবে পাঠানো হচ্ছে
            return View(prescriptionInfo.ToList());
        }

        [HttpPost]
        public IActionResult SavePrescription([FromBody] List<PrescriptionVM> prescription)
        {
            using var connection = _context.CreateConnection();
            var psychiatristName = HttpContext.Session.GetString("UserName");



            try
            {
                // ✅ Insert Prescription
                var parameters = new DynamicParameters();
                parameters.Add("@flag", 1);
                parameters.Add("@PrescriptionId", prescription.First().PrescriptionId);
                parameters.Add("@BookingId", prescription.First().BookingId); // dynamic
                parameters.Add("@Advice", prescription.First().Advice);
                //parameters.Add("@CreatedBy", "PsychiatristName"); // change as needed
                parameters.Add("@CreatedBy", psychiatristName);
                parameters.Add("@CreatedAt", DateTime.Now);

                // Assuming SP returns newly inserted PrescriptionId
                var booking = connection.QueryFirstOrDefault<PrescriptionVM>(
               "Sp_Prescription",
               parameters,
               commandType: CommandType.StoredProcedure
           );
                foreach (var item in prescription)
                {   // ✅ Insert Medicine related to that PrescriptionId
                    var parameters2 = new DynamicParameters();
                    parameters2.Add("@flag", 2);
                    //  parameters2.Add("@MedicinePrescriptionId", prescription.MedicinePrescriptionId);
                    parameters2.Add("@PrescriptionId", booking?.PrescriptionId);
                    parameters2.Add("@MedicineId", item.MedicineId);

                    // if 0, means new medicine
                    parameters2.Add("@MedicineDose", item.MedicineDose);

                    parameters2.Add("@Diagnosed", item.Diagnosed);



                    parameters2.Add("@MedicineDuration", item.MedicineDuration);
                    parameters2.Add("@Frequency", item.Frequency);
                    parameters2.Add("@Medicine_Notes", item.Medicine_Notes);

                    connection.Execute(
                        "Sp_Medicine",
                        parameters2,
                        commandType: CommandType.StoredProcedure
                    );
                }



                return Json(new { message = "Saved successfully", });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error: " + ex.Message });
            }
        }
    }
}

   
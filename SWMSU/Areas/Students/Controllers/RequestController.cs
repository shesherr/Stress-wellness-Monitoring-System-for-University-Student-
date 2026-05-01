using Dapper;
using Microsoft.AspNetCore.Mvc;
using SWMSU.Data;
using SWMSU.Models;
using System.Data;
using System.Globalization;

namespace SWMSU.Areas.Students.Controllers
{
    [Area("Students")]
    public class RequestController : Controller
    {
        private readonly DapperContext _context;

        public RequestController(DapperContext context)
        {
            _context = context;
        }

        public IActionResult RequestingViewPage()
        {
            int senderUserId = Convert.ToInt32(
                HttpContext.Session.GetString("UserId")
            );

            using (var connection = _context.CreateConnection())
            {
                var param = new DynamicParameters();
                param.Add("@Flag", 2);
                param.Add("@SenderUserId", senderUserId);

                var data = connection.Query<CounsellingRequest>(
                    "SP_CounsellingRequest",
                    param,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }
        public IActionResult Index()
        {
            int UserId = Convert.ToInt32(
                HttpContext.Session.GetString("UserId")
            );

            using (var connection = _context.CreateConnection())
            {
                var param = new DynamicParameters();
                param.Add("@Flag", 2);
                param.Add("@UserId", UserId);

                var data = connection.Query<CounsellingRequest>(
                    "Sp_BookAppointment",
                    param,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return View(data);
            }
        }
        [HttpGet]
        public IActionResult Book(int PsychiatristId)
        {
            ViewBag.PsychiatristId = PsychiatristId; // store in ViewBag
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RequestCounselling(
            int UserId,        // 👉 Psychologist UserId (form থেকে আসবে)
            string Message)
        {
            // ✅ Logged-in user (Student)
            int senderUserId = Convert.ToInt32(
       HttpContext.Session.GetString("UserId")
   );
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Flag", 1);
                parameters.Add("@SenderUserId", senderUserId); // session user
                parameters.Add("@ReceiverUserId", UserId);     // psychologist user
                parameters.Add("@Message", Message);

                await connection.ExecuteAsync(
                    "SP_CounsellingRequest",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }

            TempData["Success"] = "Request sent successfully!";
            return RedirectToAction("RequestingViewPage");
        }

        [HttpPost]
        public async Task<IActionResult> GetScheduleByAppointmentDate(GetScheduleByAppointmentDate data)
        {
            try
            {
                // Get the day name from the appointment date
                var dayName = data.AppointmentDate.Value.ToString("dddd", CultureInfo.InvariantCulture);

                // Example: call your stored procedure (replace "Sp_Something" with actual)
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PsychiatristId", data.PsychiatristId);
                    parameters.Add("@Flag", 4);

                    parameters.Add("@AppointmentDay", dayName);
                    // add other parameters as needed

                    var res = await connection.QueryFirstOrDefaultAsync(
                          "Sp_BookAppointment",
                          parameters,
                          commandType: CommandType.StoredProcedure
                      );
                    if (res != null)
                    {
                        var times = GetHourlyTimes(res.StartTime, res.Endtime, data.PsychiatristId, data.AppointmentDate, _context);
                        return Json(new { times = times, dayName = dayName });
                    }
                    return Json(null);
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }



        //    static List<TimeSchedule> GetHourlyTimes(string startTimeStr, string endTimeStr, int? userId, DateTime? appDate, DapperContext context)
        //    {
        //        List<TimeSchedule> timeList = new List<TimeSchedule>();
        //        DateTime start, end;

        //        string[] formats = { "H:mm", "HH:mm", "h:mmtt", "hh:mmtt" }; // supports 24h and 12h wit;;h AM/PM

        //        if (!DateTime.TryParseExact(startTimeStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
        //            throw new FormatException($"Invalid start time: {startTimeStr}");

        //        if (!DateTime.TryParseExact(endTimeStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
        //            throw new FormatException($"Invalid end time: {endTimeStr}");
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@PsychiatristId", userId);
        //        parameters.Add("@Flag", 12);
        //        parameters.Add("@AppointmentDate", appDate);
        //        // add other parameters as needed

        //        var bookedTimeStrings = context.CreateConnection().Query<dynamic>(
        //     "Sp_BookAppointment",
        //     parameters,
        //     commandType: CommandType.StoredProcedure
        // ).ToList();
        //        var bookedTimes = bookedTimeStrings
        //.Select(t => DateTime.ParseExact(t.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture))
        //.ToList();


        //        while (start <= end)
        //        {

        //            if (!bookedTimes.Any(t => t.TimeOfDay == start.TimeOfDay))
        //            {
        //                timeList.Add(new TimeSchedule
        //                {
        //                    StartTime = start.ToString("HH:mm"),
        //                    EndTime = start.AddHours(1).ToString("HH:mm")
        //                }); // add only if available
        //            }
        //            start = start.AddHours(1);
        //        }

        //        return timeList;
        //    }
        



        static List<TimeSchedule> GetHourlyTimes(string startTimeStr, string endTimeStr, int? userId, DateTime? appDate, DapperContext context)
        {
            List<TimeSchedule> timeList = new List<TimeSchedule>();
            DateTime start, end;

            string[] formats = { "H:mm", "HH:mm", "h:mmtt", "hh:mmtt" }; // supports 24h and 12h wit;;h AM/PM

            if (!DateTime.TryParseExact(startTimeStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                throw new FormatException($"Invalid start time: {startTimeStr}");

            if (!DateTime.TryParseExact(endTimeStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
                throw new FormatException($"Invalid end time: {endTimeStr}");
            var parameters = new DynamicParameters();
            parameters.Add("@PsychiatristId", userId);
            parameters.Add("@Flag", 12);
            parameters.Add("@AppointmentDate", appDate);
            // add other parameters as needed

            var bookedTimeStrings = context.CreateConnection().Query<dynamic>(
         "Sp_BookAppointment",
         parameters,
         commandType: CommandType.StoredProcedure
     ).ToList();
            var bookedTimes = bookedTimeStrings
    .Select(t => DateTime.ParseExact(t.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture))
    .ToList();


            while (start <= end)
            {

                if (!bookedTimes.Any(t => t.TimeOfDay == start.TimeOfDay))
                {
                    timeList.Add(new TimeSchedule
                    {
                        StartTime = start.ToString("HH:mm"),
                        EndTime = start.AddHours(1).ToString("HH:mm")
                    }); // add only if available
                }
                start = start.AddHours(1);
            }

            return timeList;
        }







        [HttpPost]
        public IActionResult Book(BookVM b)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    // Use the same flag for insert or update
                    parameters.Add("@flag", 1);
                    parameters.Add("@PsychiatristId", b.PsychiatristId);
                    parameters.Add("@UserId", Convert.ToInt32(HttpContext.Session.GetString("UserId")));
                    parameters.Add("@AppointmentDate", b.AppointmentDate);
                    parameters.Add("@AppointmentTime", b.AppointmentTime);
                    parameters.Add("@AppointmentDay", b.AppointmentDay);
                    parameters.Add("@notes", b.notes);

                    var data = connection.Execute(
                        "Sp_BookAppointment",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw;
            }
        } } }
    


    public class TimeSchedule
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }


    

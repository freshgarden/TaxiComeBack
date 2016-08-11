using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Services.Schedule;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Extension;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public partial class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly INotificationService _notificationService;

        public ScheduleController(IScheduleService scheduleService, INotificationService notificationService)
        {
            _scheduleService = scheduleService;
            _notificationService = notificationService;
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetAllSchedules()
        {
            var schedules = _scheduleService.FindSchedulesByUser(SessionPersister.UserId);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetScheduleById(Guid id)
        {
            var schedule = _scheduleService.FindScheduleById(id);
            return Json(schedule, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveScheduleInfomation(Schedule schedule)
        {
            if (!ModelState.IsValid)
                return Json(new {status = "ERROR", messenge = ModelState.Errors()}, JsonRequestBehavior.AllowGet);
            schedule.UserId = SessionPersister.UserId;
            var result = _scheduleService.SaveScheduleInformation(schedule);
            if (!result.Success)
                return Json(new {status = "ERROR", messenge = result.Errors}, JsonRequestBehavior.AllowGet);
            return Json(new {status = "OK"});
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Edit()
        {
            return View();
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateScheduleInfomation(Guid id, Schedule schedule)
        {
            if (!ModelState.IsValid)
                return Json(new {status = "ERROR", messenge = ModelState.Errors()}, JsonRequestBehavior.AllowGet);
            if (_scheduleService.FindScheduleById(id).UserId != SessionPersister.UserId)
                return Json(new {status = "ERROR", messenge = "Wrong id!"});
            schedule.UserId = SessionPersister.UserId;
            var result = _scheduleService.UpdateScheduleInformation(schedule);
            if (!result.Success)
                return Json(new {status = "ERROR", messenge = result.Errors}, JsonRequestBehavior.AllowGet);
            return Json(new {status = "OK"});
        }

        // GET: Schedule
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult GetMainSchedules()
        {
            var viewModel = new ListScheduleViewModel
            {
                Schedules = _scheduleService.FindSchedulesByUser(SessionPersister.UserId)
            };
            return PartialView(viewModel);
        }
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetScheduleEvents(string start, string end)
        {
            var schedulesForDate =
                _scheduleService.FindSchedulesByUser(SessionPersister.UserId)
                    .Where(s => s.StartDate >= ConvertFromDateTimeString(start) && s.UserId == SessionPersister.UserId);
            var eventList = from e in schedulesForDate
                            select new
                            {
                                id = e.Id,
                                title = e.BeginLocation + " - " + e.EndLocation,
                                start = e.StartDate.ToString("o"),
                                allDay = false
                            };
            var row = eventList.ToArray();
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private static DateTime ConvertFromDateTimeString(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        [HttpGet]
        public int GetCountNotifications()
        {
            var notifications =
                _notificationService.GetAllByUserId(SessionPersister.UserId).Where(x => x.Received == false).ToList();
            return notifications.Count;
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            var notifications =
                _notificationService.GetAllByUserId(SessionPersister.UserId).Where(x => x.Received == false).ToList();

            var results = notifications.Select(x => new
            {
                x.Id,
                x.CustomerFullname,
                x.CustomerPhoneNumber,
                x.NearLocation,
                x.CreateDate,
                x.Received,
                ScheduleId = x.Schedule.Id,
                ScheduleUserId = x.UserId,
                x.Schedule.BeginLocation,
                x.Schedule.EndLocation,
            }).OrderByDescending(x => x.CreateDate);

            return new JsonResult {Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
    }
}

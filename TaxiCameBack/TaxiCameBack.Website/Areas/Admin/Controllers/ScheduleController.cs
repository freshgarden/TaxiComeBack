using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Notification;
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
            var message = new GenericMessageViewModel
            {
                Message = "Save successfull!",
                MessageType = GenericMessages.success
            };
            TempData[AppConstants.MessageViewBagName] = message;
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
            var message = new GenericMessageViewModel
            {
                Message = "Update successfull!",
                MessageType = GenericMessages.success
            };
            TempData[AppConstants.MessageViewBagName] = message;
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
                _notificationService.GetAll()
                    .Where(x => x.Received == false && (x.UserId == null || x.UserId == SessionPersister.UserId))
                    .ToList();
            return notifications.Count;
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            var numberNotificationGet = 5;
            var notifications =
                _notificationService.GetAll()
                    .Where(x => x.Received == false && (x.UserId == null || x.UserId == SessionPersister.UserId))
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();

            var totalNotification = notifications.Count - numberNotificationGet;

            var results = notifications.Select(x => new
            {
                x.Id,
                x.CustomerFullname,
                x.CustomerPhoneNumber,
                x.NearLocation,
                x.CreateDate,
                x.Received,
                StartDate = x.NotificationExtend == null ? x.Schedule.StartDate : x.NotificationExtend.StartDate,
                Message = x.ScheduleId == null ? x.NotificationExtend.Message : string.Empty, 
                Type = x.NotificationExtend != null ? NotificationType.CustomerRegisted : NotificationType.DriverRegisted,
                ScheduleId = x.ScheduleId ?? Guid.Empty,
                ScheduleUserId = x.UserId ?? SessionPersister.UserId,
                BeginLocation = x.UserId != null ? x.Schedule.BeginLocation : (x.NotificationExtend != null ? x.NotificationExtend.BeginLocation : string.Empty),
                EndLocation = x.UserId != null ? x.Schedule.EndLocation : (x.NotificationExtend != null ? x.NotificationExtend.EndLocation : string.Empty),
                totalNotification,
                Total = notifications.Count
            }).Take(numberNotificationGet);

            return new JsonResult {Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
    }
}

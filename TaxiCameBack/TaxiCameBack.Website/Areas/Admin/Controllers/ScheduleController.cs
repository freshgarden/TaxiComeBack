using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Services.Schedule;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Extension;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public partial class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetAllSchedules()
        {
            var schedules = _scheduleService.FindSchedules();
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetScheduleById(int id)
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
        [ValidateAntiForgeryToken]
        public JsonResult SaveScheduleInfomation(Schedule schedule)
        {
            if (!ModelState.IsValid)
                return Json(new {status = "ERROR", messenge = ModelState.Errors()}, JsonRequestBehavior.AllowGet);
            var result = _scheduleService.SaveScheduleInformation(schedule);
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
                Schedules = _scheduleService.FindSchedules()
            };
            return PartialView(viewModel);
        }
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public JsonResult GetScheduleEvents(string start, string end)
        {
            var schedulesForDate =
                _scheduleService.FindSchedules().Where(s => s.StartDate >= ConvertFromDateTimeString(start));
            var eventList = from e in schedulesForDate
                            select new
                            {
                                id = e.Id,
                                title = e.BeginLocation + " - " + e.EndLocation,
                                start = e.StartDate.ToString(""),
                                allDay = false
                            };
            var row = eventList.ToArray();
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private static DateTime ConvertFromDateTimeString(string date)
        {
            return DateTime.ParseExact("2012-04-05", "yyyy-MM-dd", CultureInfo.InvariantCulture); ;
        }
    }
}

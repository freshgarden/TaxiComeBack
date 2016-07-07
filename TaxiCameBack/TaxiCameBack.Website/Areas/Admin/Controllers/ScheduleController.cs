using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Services.Schedule;
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

        public JsonResult GetAllSchedules()
        {
            var schedules = _scheduleService.FindSchedules();
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScheduleById(int id)
        {
            var schedule = _scheduleService.FindScheduleById(id);
            return Json(schedule, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateEdit()
        {
            return View();
        }
        
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

        public JsonResult GetScheduleEvents(double start, double end)
        {
            var schedulesForDate =
                _scheduleService.FindSchedules().Where(s => s.StartDate >= ConvertFromUnixTimestamp(start));
            var eventList = from e in schedulesForDate
                select new
                {
                    id = e.Id,
                    title = e.BeginLocation + " - " + e.EndLocation,
                    start = e.StartDate.ToString("s"),
                    allDay = false
                };
            var row = eventList.ToArray();
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}

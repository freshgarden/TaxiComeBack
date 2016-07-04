using System.Linq;
using System.Net;
using System.Web.Mvc;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Services.Schedule;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public partial class ScheduleController : Controller
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
        
        public HttpStatusCodeResult SaveScheduleInfomation(Schedule schedule)
        {
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ModelState.Values.SelectMany(v => v.Errors).ToList().ToString());
            var result = _scheduleService.SaveScheduleInformation(schedule);
            if (!result.Success)
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed, result.Errors.ToString());
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // GET: Schedule
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateEdit()
        {
            return View();
        }
    }
}

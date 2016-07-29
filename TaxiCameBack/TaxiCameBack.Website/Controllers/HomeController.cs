using System.Linq;
using System.Web.Mvc;
using TaxiCameBack.MapUtilities;
using TaxiCameBack.Services.Search;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchSchduleService _searchSchduleService;

        public HomeController(ISearchSchduleService searchSchduleService)
        {
            _searchSchduleService = searchSchduleService;
        }

        [HttpPost]
        public JsonResult Search(SearchModel searchModel)
        {
            var schedules = _searchSchduleService.Search(
                new PointLatLng(searchModel.StartLocationLat, searchModel.StartLocationLng),
                new PointLatLng(searchModel.EndLocationLat, searchModel.EndLocationLng),
                searchModel.CarType,
                searchModel.StartDate);

            var results = schedules.Select(schedule => new ResultSearchModel
            {
                BeginLocation = schedule.BeginLocation,
                EndLocation = schedule.EndLocation,
                StartDate = schedule.StartDate,
                ScheduleGeolocations = schedule.ScheduleGeolocations,
                UserFullName = schedule.User.FullName,
                UserGender = schedule.User.PhoneNumber,
                UserPhoneNumber = schedule.User.PhoneNumber,
                UserAvatar = schedule.User.Avatar,
                UserCarNumber = schedule.User.CarNumber,
                UserCarmakers = schedule.User.Carmakers,
                DriveId = schedule.UserId
            }).ToList();

            return schedules.Count == 0
                ? Json(new { }, JsonRequestBehavior.AllowGet)
                : Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CustomerRegisterCar(CustomerRegisterCar customerRegisterCar)
        {
            return Json(new { },JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
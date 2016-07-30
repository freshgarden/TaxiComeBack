using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaxiCameBack.MapUtilities;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Search;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchSchduleService _searchSchduleService;
        private readonly IMembershipService _membershipService;

        public HomeController(ISearchSchduleService searchSchduleService, IMembershipService membershipService)
        {
            _searchSchduleService = searchSchduleService;
            _membershipService = membershipService;
        }

        [HttpPost]
        public JsonResult Search(SearchModel searchModel)
        {
            var schedules = _searchSchduleService.Search(
                new PointLatLng(searchModel.StartLocationLat, searchModel.StartLocationLng),
                new PointLatLng(searchModel.EndLocationLat, searchModel.EndLocationLng),
                searchModel.CarType,
                searchModel.StartDate);
            
            var results = (from schedule in schedules
                let user = _membershipService.GetById(schedule.UserId)
                select new ResultSearchModel
                {
                    ScheduleId = schedule.Id,
                    BeginLocation = schedule.BeginLocation,
                    EndLocation = schedule.EndLocation,
                    StartDate = schedule.StartDate,
                    ScheduleGeolocations = schedule.ScheduleGeolocations,
                    UserFullName = user.FullName,
                    UserGender = user.Gender,
                    UserPhoneNumber = user.PhoneNumber,
                    UserAvatar = user.Avatar,
                    UserCarNumber = user.CarNumber,
                    UserCarmakers = user.Carmakers,
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
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TaxiCameBack.DTO.Schedule;
using TaxiCameBack.MapUtilities;
using TaxiCameBack.Services.Search;

namespace TaxiCameBack.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchSchduleService _searchSchduleService;

        public HomeController(ISearchSchduleService searchSchduleService)
        {
            _searchSchduleService = searchSchduleService;
        }

        public JsonResult Search(string startLocationLat, string startLocationLng, string endLocationLat, string endLocationLng, string carType)
        {
            //var schedules = _searchSchduleService.Search(
            //                    new PointLatLng(Convert.ToDouble(startLocationLat), Convert.ToDouble(startLocationLng)),
            //                    new PointLatLng(Convert.ToDouble(endLocationLat), Convert.ToDouble(endLocationLng)));
            
            return Json(new { }, JsonRequestBehavior.AllowGet);
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
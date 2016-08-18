using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.DomainModel.Notification;
using TaxiCameBack.MapUtilities;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Services.Search;
using TaxiCameBack.Website.Application.Extension;
using TaxiCameBack.Website.Application.Recaptcha;
using TaxiCameBack.Website.Application.Signalr;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchSchduleService _searchSchduleService;
        private readonly IMembershipService _membershipService;
        private readonly INotificationService _notificationService;

        public HomeController(ISearchSchduleService searchSchduleService, IMembershipService membershipService, INotificationService notificationService)
        {
            _searchSchduleService = searchSchduleService;
            _membershipService = membershipService;
            _notificationService = notificationService;
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

        [CaptchaValidator]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerRegisterCar(CustomerRegistrationNewScheule newScheule)
        {
            if (!ModelState.IsValid)
                return
                    Json(
                        new
                        {
                            Status = "ERROR",
                            Message = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList()[0]
                        });
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var validate = NotificationHub.ValidateCustomerRegistrationNewShedule(newScheule);
            if (!validate.Success)
            {
                return Json(new {Status = "ERROR", Message = validate.Errors[0]});
            }

            DateTime sDate;
            if (!string.IsNullOrEmpty(newScheule.StartDateHidden))
            {
                if (!DateTime.TryParse(newScheule.StartDateHidden, out sDate))
                {
                    return Json(new {Status = "ERROR", Message = "Invalid start date."});
                }
            }
            else
            {
                sDate = DateTime.UtcNow;
            }
            var notification = new Notification
            {
                CustomerFullname = newScheule.CustomerFullName,
                CustomerPhoneNumber = newScheule.CustomerPhoneNumber,
                NearLocation = newScheule.NearLocation
            };
            var notificationExtend = new NotificationExtend
            {
                BeginLocation = newScheule.BeginLocation,
                EndLocation = newScheule.EndLocation,
                StartDate = sDate,
                Message = newScheule.Message
            };
            var result = _notificationService.CreateNewSchedule(notification, notificationExtend);
            if (!result.Success)
            {
                return Json(new { Status = "ERROR", Message = result.Errors[0] });
            }
            context.Clients.Clients(NotificationHub.Connections.ToList()).updateReceived();
            return Json(new {Status = "OK", Message = "Create schedule successfull! Please wait driver contact with you." },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View(new CustomerRegistrationNewScheule());
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
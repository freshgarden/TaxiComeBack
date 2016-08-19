using System;
using System.Web.Mvc;
using System.Web.Routing;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Areas.Admin.Models;
using TaxiCameBack.Website.Areas.Admin.Models.Mapping;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Index()
        {
            return ListNotification(null, null);
        }
        [HttpPost]
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Filter(NotificationSearchModel searchModel, string command)
        {
            if (command == "Reset")
                return RedirectToAction("Index");
            return ListNotification(null, searchModel);
        }
        
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        private ActionResult ListNotification(int? p, NotificationSearchModel search)
        {
            if (search != null && search.Day > 0 && search.Month > 0 && search.Year > 0)
                search.StartDate = new DateTime(search.Year, search.Month, search.Day);
            var notiSearch = ViewModelMapping.SearchModelToDomainSearchModel(search);
            var pageIndex = p ?? 1;
            var allNotification = _notificationService.GetAllPaged(SessionPersister.UserId, pageIndex, AppConstants.AdminListPageSize, notiSearch);

            // Redisplay list of users
            var notificationListViewModel = new NotificationListViewModel
            {
                Notifications = allNotification,
                PageIndex = pageIndex,
                TotalCount = allNotification.Count,
                TotalPages = allNotification.TotalPages,
                NotificationSearchModel = search ?? new NotificationSearchModel()
            };
            return View("Index", notificationListViewModel);
        }
    }
}
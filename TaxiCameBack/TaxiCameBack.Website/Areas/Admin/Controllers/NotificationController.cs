using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Areas.Admin.Models;

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
        public ActionResult Index(int? p, string search)
        {
            return ListNotification(p, search);
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        private ActionResult ListNotification(int? p, string search)
        {
            var pageIndex = p ?? 1;
            var allNotification = _notificationService.GetAllByUserId(SessionPersister.UserId, pageIndex, AppConstants.AdminListPageSize);

            // Redisplay list of users
            var memberListModel = new NotificationListViewModel
            {
                Notifications = allNotification,
                PageIndex = pageIndex,
                Search = search,
                TotalCount = allNotification.Count,
                TotalPages = allNotification.TotalPages
            };

            return View("Index", memberListModel);
        }
    }
}
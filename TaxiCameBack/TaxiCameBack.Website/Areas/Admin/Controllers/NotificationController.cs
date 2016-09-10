using System;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Application.Signalr;
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
            if (command == "Xóa điều kiện")
                return RedirectToAction("Index");
            return ListNotification(null, searchModel);
        }
        
        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        private ActionResult ListNotification(int? p, NotificationSearchModel search)
        {
            if (!string.IsNullOrEmpty(search?.StartDateString))
            {
                var datetime = search.StartDateString.Split('-');
                if (datetime.Length == 3)
                {
                    int checkIsInt;
                    if (int.TryParse(datetime[0], out checkIsInt) && datetime[0].ToInt32() > 0 && datetime[0].ToInt32() < 31)
                    {
                        if (int.TryParse(datetime[1], out checkIsInt) && datetime[1].ToInt32() > 0 && datetime[1].ToInt32() < 12)
                        {
                            if (int.TryParse(datetime[2], out checkIsInt) && datetime[2].ToInt32() > 2015 &&
                                datetime[2].ToInt32() <= DateTime.Now.Year)
                            {
                                search.StartDate = new DateTime(datetime[2].ToInt32(), datetime[1].ToInt32(), datetime[0].ToInt32());
                            }
                    }
                    }
                }
            }
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

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Cancel(Guid id)
        {
            var notification = _notificationService.GetById(id);
            var error = string.Empty;
            if (notification.IsCancel)
            {
                error = "Cannot cancel notification which was cancel.";
                var message = new GenericMessageViewModel
                {
                    Message = error,
                    MessageType = GenericMessages.success
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }
            if (notification.NotificationExtend == null)
            {
                error = "Cannot cancel notification which is driver registed.";
                var message = new GenericMessageViewModel
                {
                    Message = error,
                    MessageType = GenericMessages.success
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }

            var result = _notificationService.Cancel(notification);
            if (!result.Success)
            {
                error = "Error Cancel notification.";
                var message = new GenericMessageViewModel
                {
                    Message = error,
                    MessageType = GenericMessages.success
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Clients(NotificationHub.Connections.ToList()).updateReceived();
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = "Cancel success.",
                MessageType = GenericMessages.success
            };

            return RedirectToAction("Index");
        }
    }
}
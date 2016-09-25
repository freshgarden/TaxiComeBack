using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services;
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
            if (command == "Bỏ điều kiện")
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
            
            if (notification == null)
            {
                var message = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_not_found,
                    MessageType = GenericMessages.danger
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }
            if (notification.IsCancel)
            {
                var message = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_cancel_was_cancel,
                    MessageType = GenericMessages.danger
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }
            CrudResult result;

            if (notification.Schedule == null && notification.NotificationExtend != null)
            {
                notification.Received = false;
                notification.UserId = null;
                result = _notificationService.Update(notification);
            }
            else
            {
                result = _notificationService.Cancel(notification);
            }
            
            if (!result.Success)
            {
                var message = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_cancel_err,
                    MessageType = GenericMessages.danger
                };
                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index");
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Clients(NotificationHub.Connections.ToList()).updateReceived();
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = App_LocalResources.Notifications.notification_cancel_suc,
                MessageType = GenericMessages.success
            };

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = AppConstants.StandardMembers)]
        public ActionResult Received(Guid id)
        {
            var notification = _notificationService.GetById(id);
            if (notification == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_not_found,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction("Index");
            }
            if (notification.Received)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_receive_was_receive,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction("Index");
            }
            if (notification.Schedule != null && notification.Schedule.Notifications.Any(x => x.Received && !x.IsCancel))
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_receive_was_receive,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction("Index");
            }
            if (notification.IsCancel)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_cancel,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction("Index");
            }

            if (notification.Schedule != null && notification.Schedule.UserId != SessionPersister.UserId)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_not_receive_other,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction("Index");
            }
            notification.UserId = SessionPersister.UserId;
            var result = notification.Schedule == null && notification.NotificationExtend != null
                ? _notificationService.UpdateRecieved(notification)
                : _notificationService.UpdateRecieved(notification.Schedule.Id, notification.Id);

            if (!result.Success)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.Notifications.notification_receive_err,
                    MessageType = GenericMessages.danger
                };
                return RedirectToAction("Index");
            }
            if (notification.Schedule == null && notification.NotificationExtend != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.Clients(NotificationHub.Connections.ToList()).updateReceived();
            }
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = App_LocalResources.Notifications.notification_receive_suc,
                MessageType = GenericMessages.success
            };

            return RedirectToAction("Index");
        }
    }
}
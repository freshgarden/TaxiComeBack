using System;
using System.Collections.Generic;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Logging;

namespace TaxiCameBack.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Core.DomainModel.Notification.Notification> _notificationRepository;
        private readonly ILoggingService _loggingService;

        public NotificationService(IRepository<Core.DomainModel.Notification.Notification> notificationRepository, ILoggingService loggingService)
        {
            _notificationRepository = notificationRepository;
            _loggingService = loggingService;
        }

        public CrudResult Create(Core.DomainModel.Notification.Notification notification)
        {
            var result = new CrudResult();
            notification.Message = StringUtils.SafePlainText(notification.Message);
            notification.CreateDate = DateTime.UtcNow;

            try
            {
                _notificationRepository.Insert(notification);
                _notificationRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                result.AddError(exception.Message);
            }
            return result;
        }

        public List<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId)
        {
            var notifications =
                _notificationRepository.FindBy(x => x.UserId == userId && x.Received == false)
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
            return notifications;
        }
    }
}

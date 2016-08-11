using System;
using System.Collections.Generic;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.PagedList;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Data;
using TaxiCameBack.Data.Contract;
using TaxiCameBack.Services.Logging;

namespace TaxiCameBack.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Core.DomainModel.Notification.Notification> _notificationRepository;
        private IQueryableUnitOfWork _unitOfWork;
        private readonly ILoggingService _loggingService;

        public NotificationService(IRepository<Core.DomainModel.Notification.Notification> notificationRepository, IQueryableUnitOfWork unitOfWork, ILoggingService loggingService)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _loggingService = loggingService;
        }

        public CrudResult Create(Core.DomainModel.Notification.Notification notification)
        {
            var result = new CrudResult();
            notification.CustomerFullname = StringUtils.SafePlainText(notification.CustomerFullname);
            notification.CustomerPhoneNumber = StringUtils.SafePlainText(notification.CustomerPhoneNumber);
            notification.NearLocation = StringUtils.SafePlainText(notification.NearLocation);
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

        public Core.DomainModel.Notification.Notification GetById(Guid notificationId)
        {
            return _notificationRepository.GetById(notificationId);
        }

        public List<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId)
        {
            var notifications =
                _notificationRepository.FindBy(x => x.UserId == userId && x.Received == false)
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
            return notifications;
        }

        public PagedList<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId, int pageIndex, int pageSize)
        {
            var totalCount = ((EfUnitOfWork)_unitOfWork).Notification.Count();
            var results = ((EfUnitOfWork)_unitOfWork).Notification
                                .Where(x => x.UserId == userId)
                                .OrderByDescending(x => x.Received)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new PagedList<Core.DomainModel.Notification.Notification>(results, pageIndex, pageSize, totalCount);
        }

        public CrudResult Update(Core.DomainModel.Notification.Notification notification)
        {
            var result = new CrudResult();
            notification.CustomerFullname = StringUtils.SafePlainText(notification.CustomerFullname);
            notification.CustomerPhoneNumber = StringUtils.SafePlainText(notification.CustomerPhoneNumber);
            notification.NearLocation = StringUtils.SafePlainText(notification.NearLocation);
            var oldNotification = _notificationRepository.GetById(notification.Id);
            try
            {
                _notificationRepository.Merge(notification, oldNotification);
                _notificationRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                result.AddError(exception.Message);
                _notificationRepository.UnitOfWork.Rollback();
            }
            return result;
        }
    }
}

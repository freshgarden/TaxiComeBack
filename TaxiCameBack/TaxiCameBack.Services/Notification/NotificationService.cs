using System;
using System.Collections.Generic;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Notification;
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
        private readonly IRepository<NotificationExtend> _notificationExtendRepository;
        private readonly IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;
        private readonly IQueryableUnitOfWork _unitOfWork;
        private readonly ILoggingService _loggingService;

        public NotificationService(IRepository<Core.DomainModel.Notification.Notification> notificationRepository,
            IRepository<Core.DomainModel.Schedule.Schedule> scheduleRepository, IQueryableUnitOfWork unitOfWork,
            ILoggingService loggingService, IRepository<NotificationExtend> notificationExtendRepository)
        {
            _notificationRepository = notificationRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _loggingService = loggingService;
            _notificationExtendRepository = notificationExtendRepository;
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

        public CrudResult CreateNewSchedule(Core.DomainModel.Notification.Notification notification, NotificationExtend notificationExtend)
        {
            var result = new CrudResult();
            notification.CustomerFullname = StringUtils.SafePlainText(notification.CustomerFullname);
            notification.CustomerPhoneNumber = StringUtils.SafePlainText(notification.CustomerPhoneNumber);
            notification.NearLocation = StringUtils.SafePlainText(notification.NearLocation);
            notification.Received = false;
            notification.CreateDate = DateTime.UtcNow;

            notificationExtend.BeginLocation = StringUtils.SafePlainText(notificationExtend.BeginLocation);
            notificationExtend.EndLocation = StringUtils.SafePlainText(notificationExtend.EndLocation);
            notificationExtend.Message = StringUtils.SafePlainText(notificationExtend.Message);
            if (notificationExtend.StartDate < DateTime.UtcNow)
            {
                notificationExtend.StartDate = DateTime.UtcNow;
            }
            notificationExtend.Notification = notification;

            try
            {
                _notificationRepository.Insert(notification);
                _notificationExtendRepository.Insert(notificationExtend);
                _notificationRepository.UnitOfWork.Commit();
                _notificationExtendRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                result.AddError(exception.Message);
                _notificationRepository.UnitOfWork.Rollback();
                _notificationExtendRepository.UnitOfWork.Rollback();
            }

            return result;
        }

        public List<Core.DomainModel.Notification.Notification> GetAll()
        {
            return ((EfUnitOfWork)_unitOfWork).Notification.ToList();
        }

        public PagedList<Core.DomainModel.Notification.Notification> GetAllPaged(Guid userId, int pageIndex, int pageSize)
        {
            var totalCount = ((EfUnitOfWork)_unitOfWork).Notification.Count();
            var results = ((EfUnitOfWork)_unitOfWork).Notification
                                .Where(x => x.UserId == userId || x.UserId == null)
                                .OrderByDescending(x => x.Received)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new PagedList<Core.DomainModel.Notification.Notification>(results, pageIndex, pageSize, totalCount);
        }

        public PagedList<Core.DomainModel.Notification.Notification> GetAllPaged(Guid userId, int pageIndex, int pageSize, NotificationSearchModel searchModel)
        {
            var totalCount = ((EfUnitOfWork)_unitOfWork).Notification.Count();
            var results = ((EfUnitOfWork) _unitOfWork).Notification
                .Where(x => x.UserId == userId || x.UserId == null)
                .OrderByDescending(x => x.Received)
                .Skip((pageIndex - 1)*pageSize)
                .Take(pageSize);

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.CustomerFullname))
                    results = results.Where(x => x.CustomerFullname.Contains(searchModel.CustomerFullname));
                if (!string.IsNullOrEmpty(searchModel.CustomerPhoneNumber))
                    results = results.Where(x => x.CustomerPhoneNumber.Contains(searchModel.CustomerPhoneNumber));
                if (!string.IsNullOrEmpty(searchModel.NearLocation))
                    results = results.Where(x => x.NearLocation.Contains(searchModel.NearLocation));
                if (searchModel.Received != null)
                    results = results.Where(x => x.Received == searchModel.Received);
                if (searchModel.StartDate != null)
                {
                    var resultList = new List<Core.DomainModel.Notification.Notification>();
                    foreach (var result in results.ToList())
                    {
                        if (result.ScheduleId != null)
                        {
                            if (result.Schedule.StartDate.Date == searchModel.StartDate.Value.Date
                                && result.Schedule.StartDate.Month == searchModel.StartDate.Value.Month
                                && result.Schedule.StartDate.Year == searchModel.StartDate.Value.Year)
                            {
                                resultList.Add(result);
                            }
                        }
                        else
                        {
                            if (result.NotificationExtend.StartDate.Date == searchModel.StartDate.Value.Date
                                && result.NotificationExtend.StartDate.Month == searchModel.StartDate.Value.Month
                                && result.NotificationExtend.StartDate.Year == searchModel.StartDate.Value.Year)
                            {
                                resultList.Add(result);
                            }
                        }
                    }
                    results = resultList.AsQueryable();
                }
            }

            return new PagedList<Core.DomainModel.Notification.Notification>(results.ToList(), pageIndex, pageSize, totalCount);
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

        public List<Core.DomainModel.Notification.Notification> GetAllByScheduleId(Guid scheduleId)
        {
            var notifications = _notificationRepository.FindBy(x => x.ScheduleId == scheduleId).ToList();
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

        public CrudResult UpdateRecieved(Guid scheduleId)
        {
            var result = new CrudResult();

            var notifications =
                _notificationRepository.FindBy(x => x.ScheduleId == scheduleId && x.Received == false).ToList();
            notifications.ForEach(x =>
            {
                x.Received = true;
                x.ReceivedDate = DateTime.UtcNow;
            });

            try
            {
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

        public CrudResult UpdateRecieved(Core.DomainModel.Notification.Notification notification)
        {
            var result = new CrudResult();

            notification.CustomerFullname = StringUtils.SafePlainText(notification.CustomerFullname);
            notification.CustomerPhoneNumber = StringUtils.SafePlainText(notification.CustomerPhoneNumber);
            notification.NearLocation = StringUtils.SafePlainText(notification.NearLocation);

            notification.Received = true;
            notification.ReceivedDate = DateTime.UtcNow;
            var oldNotification = _notificationRepository.GetById(notification.Id);

            try
            {
                _notificationRepository.Merge(oldNotification, notification);
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

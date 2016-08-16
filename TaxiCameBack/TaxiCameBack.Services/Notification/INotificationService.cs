using System;
using System.Collections.Generic;
using TaxiCameBack.Core.DomainModel.Notification;
using TaxiCameBack.Core.DomainModel.PagedList;

namespace TaxiCameBack.Services.Notification
{
    public interface INotificationService
    {
        CrudResult Create(Core.DomainModel.Notification.Notification notification);
        CrudResult CreateNewSchedule(Core.DomainModel.Notification.Notification notification, NotificationExtend notificationExtend);
        List<Core.DomainModel.Notification.Notification> GetAll();
        PagedList<Core.DomainModel.Notification.Notification> GetAllPaged(Guid userId, int pageIndex, int pageSize);
        Core.DomainModel.Notification.Notification GetById(Guid notificationId);
        List<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId);
        List<TaxiCameBack.Core.DomainModel.Notification.Notification> GetAllByScheduleId(Guid scheduleId);
        PagedList<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId, int pageIndex, int pageSize);
        CrudResult Update(Core.DomainModel.Notification.Notification notification);
        CrudResult UpdateRecieved(Guid scheduleId);
        CrudResult UpdateRecieved(Core.DomainModel.Notification.Notification notification);
    }
}

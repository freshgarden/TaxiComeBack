using System;
using System.Collections.Generic;
using TaxiCameBack.Core.DomainModel.PagedList;

namespace TaxiCameBack.Services.Notification
{
    public interface INotificationService
    {
        CrudResult Create(Core.DomainModel.Notification.Notification notification);
        TaxiCameBack.Core.DomainModel.Notification.Notification GetById(Guid notificationId);
        List<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId);
        PagedList<Core.DomainModel.Notification.Notification> GetAllByUserId(Guid userId, int pageIndex, int pageSize);
        CrudResult Update(Core.DomainModel.Notification.Notification notification);
    }
}

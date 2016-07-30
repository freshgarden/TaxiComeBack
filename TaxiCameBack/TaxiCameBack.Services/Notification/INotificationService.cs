using System.Collections.Generic;

namespace TaxiCameBack.Services.Notification
{
    public interface INotificationService
    {
        CrudResult Create(Core.DomainModel.Notification.Notification notification);

        List<Core.DomainModel.Notification.Notification> GetAllByUserId(int userId);
    }
}

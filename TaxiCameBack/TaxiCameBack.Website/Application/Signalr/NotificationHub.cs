using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Notification;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Services.Schedule;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Application.Signalr
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;
        private readonly IScheduleService _scheduleService;
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        
        public NotificationHub(
            INotificationService notificationService, 
            IMembershipService membershipService,
            IScheduleService scheduleService)
        {
            _notificationService = notificationService;
            _membershipService = membershipService;
            _scheduleService = scheduleService;
        }

        public void AddDriver(string username)
        {
            username = StringUtils.SafePlainText(username);
            if (!string.IsNullOrEmpty(username))
            {
                _connections.Remove(username);
                _connections.Add(username, Context.ConnectionId);
            }
        }

        public void RegisterTaxi(CustomerRegisterCar customer)
        {
            var driver = _membershipService.GetById(customer.DriveId);
            if (driver == null)
                return;

            var schedule = _scheduleService.FindSchedulesByUser(driver.UserId).FirstOrDefault(s => s.Id == customer.ScheduleId);
            if (schedule == null)
                return;

            var result = _notificationService.Create(new Notification
            {
                UserId = driver.UserId,
                Message = string.Format(AppConstants.NotificationMessage, customer.CustomerFullName,
                    customer.CustomerPhoneNumber, schedule.BeginLocation, schedule.EndLocation,
                    schedule.StartDate.ToString("dd-MM-yyyy")),
                Received = false,
                Schedule = schedule
            });

            if (!result.Success)
            {
                return;
            }
            
            var connectionId = _connections.GetConnections(driver.Email);
            Clients.Client(connectionId).addRegisted();
        }

        public void Viewed()
        {
            
        }

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if(_connections.Count > 0)
                _connections.RemoveByValue(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
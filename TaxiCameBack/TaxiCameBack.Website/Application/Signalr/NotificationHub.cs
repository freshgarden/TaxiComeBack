using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
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

            if (string.IsNullOrEmpty(customer.CustomerFullName))
            {
                Clients.Caller.addNewMessageToPage("Full name can't be blank.");
                return;
            }

            if (string.IsNullOrEmpty(customer.CustomerPhoneNumber))
            {
                Clients.Caller.addNewMessageToPage("Phone number can't be blank.");
                return;
            }

            var result = _notificationService.Create(new Notification
            {
                UserId = driver.UserId,
                CustomerFullname = customer.CustomerFullName,
                CustomerPhoneNumber = customer.CustomerPhoneNumber,
                Received = false,
                Schedule = schedule
            });

            if (!result.Success)
            {
                Clients.Caller.addNewMessageToPage(result.Errors[0]);
                return;
            }
            
            var connectionId = _connections.GetConnections(driver.Email);
            Clients.Client(connectionId).addRegisted("Registed taxi successful. Please wait diver contact with you.");
        }

        public void Received(Guid notificationId, Guid scheduleId, Guid driverId)
        {
            var driver = _membershipService.GetById(driverId);
            if (driver == null)
            {
                return;
            }
            var connectionId = _connections.GetConnections(driver.Email);
            var schedule = _scheduleService.FindScheduleById(scheduleId);
            if (schedule == null)
            {
                UpdateDriverResult(connectionId, "Schedule is not exist.");
                return;
            }

            if (schedule.Notification.Received)
            {
                UpdateDriverResult(connectionId, "Schedule was received.");
                return;
            }

            if (schedule.UserId != driverId)
            {
                UpdateDriverResult(connectionId, "You cannot have permission to access this schedule.");
                return;
            }

            var notification = _notificationService.GetById(notificationId);
            if (notification == null)
            {
                UpdateDriverResult(connectionId, "This notification is not exist.");
                return;
            }

            notification.Received = true;
            notification.ReceivedDate = DateTime.UtcNow;
            var result = _notificationService.Update(notification);
            if (!result.Success)
            {
                UpdateDriverResult(connectionId, "Receive fail: " + result.Errors[0]);
                return;
            }
            
            Clients.Client(connectionId).updateReceived();
        }

        private void UpdateDriverResult(string connectionId, string message)
        {
            Clients.Client(connectionId).updateResult(message);
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
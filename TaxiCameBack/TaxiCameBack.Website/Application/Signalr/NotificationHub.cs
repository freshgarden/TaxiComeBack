using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Application.Signalr
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public NotificationHub(INotificationService notificationService, IMembershipService membershipService)
        {
            _notificationService = notificationService;
            _membershipService = membershipService;
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
            var connectionId = _connections.GetConnections(driver.Email);
            Clients.Client(connectionId).addRegisted("dsadasdsadas");
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
            _connections.RemoveByValue(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
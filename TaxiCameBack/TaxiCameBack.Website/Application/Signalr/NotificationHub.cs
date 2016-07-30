using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Notification;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Application.Signalr
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;
        
        public NotificationHub(INotificationService notificationService, IMembershipService membershipService)
        {
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        public override Task OnConnected()
        {
            var s = "dasdas";
            return base.OnConnected();
        }

        public void RegisterTaxi(CustomerRegisterCar customer)
        {
            var driver = _membershipService.GetById(customer.DriveId);
            
            Clients.User(driver.Email).send("dsadasdsadas");
        }

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var s = "dsadas";
            return base.OnDisconnected(stopCalled);
        }
    }
}
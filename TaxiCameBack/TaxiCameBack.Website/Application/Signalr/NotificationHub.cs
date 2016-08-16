using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TaxiCameBack.Core.DomainModel.Notification;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services;
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
        public static readonly ConnectionMapping<string> Connections = new ConnectionMapping<string>();
        
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
                Connections.Remove(username);
                Connections.Add(username, Context.ConnectionId);
            }
        }

        /// <summary>
        /// Customer register taxi.
        /// </summary>
        /// <param name="customer"></param>
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
                Clients.Caller.addNewErrorMessageToPage("Full name can't be blank.");
                return;
            }

            if (string.IsNullOrEmpty(customer.CustomerPhoneNumber))
            {
                Clients.Caller.addNewErrorMessageToPage("Phone number can't be blank.");
                return;
            }

            if (string.IsNullOrEmpty(customer.NearLocation))
            {
                Clients.Caller.addNewErrorMessageToPage("Near location can't be blank.");
            }

            var result = _notificationService.Create(new Notification
            {
                UserId = driver.UserId,
                CustomerFullname = customer.CustomerFullName,
                CustomerPhoneNumber = customer.CustomerPhoneNumber,
                NearLocation = customer.NearLocation,
                Received = false,
                Schedule = schedule
            });

            if (!result.Success)
            {
                Clients.Caller.addNewErrorMessageToPage(result.Errors[0]);
                return;
            }
            
            var connectionId = Connections.GetConnections(driver.Email);
            if (!string.IsNullOrEmpty(connectionId))
            {
                Clients.Client(connectionId).addRegisted();
            }
            Clients.Caller.addNewMessageToPage("Registed taxi successful. Please wait diver contact with you.");
        }

        /// <summary>
        /// Driver receive
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="scheduleId"></param>
        /// <param name="driverId"></param>
        public void Received(Guid notificationId, Guid scheduleId, Guid driverId)
        {
            if (scheduleId != Guid.Empty)
            {
                ReceivedCustomerRegistration(notificationId, scheduleId, driverId);
            }
            ReceivedCustomerRegistrationNewSchedule(notificationId, driverId);
        }

        /// <summary>
        /// Driver received schedule which driver registration
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="scheduleId"></param>
        /// <param name="driverId"></param>
        private void ReceivedCustomerRegistration(Guid notificationId, Guid scheduleId, Guid driverId)
        {
            var driver = _membershipService.GetById(driverId);
            if (driver == null)
            {
                return;
            }
            var connectionId = Connections.GetConnections(driver.Email);
            var schedule = _scheduleService.FindScheduleById(scheduleId);
            if (schedule == null)
            {
                UpdateDriverResult(connectionId, "Schedule is not exist.");
                return;
            }

            if (schedule.Notifications.Any(x => x.Received))
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

            var allNotification = _notificationService.GetAllByScheduleId(scheduleId);
            if (allNotification.Where(x => x.Received).ToList().Count > 0)
            {
                UpdateDriverResult(connectionId, "This schedule was received.");
                return;
            }
            var result = _notificationService.UpdateRecieved(scheduleId);
            if (!result.Success)
            {
                UpdateDriverResult(connectionId, "Receive fail: " + result.Errors[0]);
                return;
            }

            Clients.Client(connectionId).updateReceived();
        }

        /// <summary>
        /// Driver received the new schedule which customer registration.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="driverId"></param>
        private void ReceivedCustomerRegistrationNewSchedule(Guid notificationId, Guid driverId)
        {
            var driver = _membershipService.GetById(driverId);
            if (driver == null)
            {
                return;
            }
            var connectionId = Connections.GetConnections(driver.Email);
            var notification = _notificationService.GetById(notificationId);
            if (notification == null)
            {
                UpdateDriverResult(connectionId, "This notification is not exist.");
                return;
            }

            notification.UserId = driverId;
            var result = _notificationService.UpdateRecieved(notification);
            if (!result.Success)
            {
                UpdateDriverResult(connectionId, "Receive fail: " + result.Errors[0]);
                return;
            }
            Clients.Clients(Connections.ToList()).updateReceived();
        }

        private void UpdateDriverResult(string connectionId, string message)
        {
            Clients.Client(connectionId).updateResult(message);
        }

        /// <summary>
        /// Customer register new schedule.
        /// </summary>
        /// <param name="newScheule"></param>
//        public void RegistrationNewSchedule(CustomerRegistrationNewScheule newScheule)
//        {
//            var validate = ValidateCustomerRegistrationNewShedule(newScheule);
//            if (!validate.Success)
//            {
//                Clients.Caller.addNewErrorMessageToPage(validate.Errors[0]);
//                return;
//            }
//            var notification = new Notification
//            {
//                CustomerFullname = newScheule.CustomerPhoneNumber,
//                CustomerPhoneNumber = newScheule.CustomerPhoneNumber,
//                NearLocation = newScheule.NearLocation
//            };
//            var notificationExtend = new NotificationExtend
//            {
//                BeginLocation = newScheule.BeginLocation,
//                EndLocation = newScheule.EndLocation,
//                StartDate = newScheule.StartDate,
//                Message = newScheule.Message
//            };
//            var result = _notificationService.CreateNewSchedule(notification, notificationExtend);
//            if (!result.Success)
//            {
//                Clients.Caller.addNewErrorMessageToPage(result.Errors[0]);
//                return;
//            }
//            Clients.Clients(Connections.ToList()).updateReceived();
//            Clients.Caller.addNewMessageToPage("Create schedule successfull! Please wait driver contact with you.");
//        }

        public static CrudResult ValidateCustomerRegistrationNewShedule(CustomerRegistrationNewScheule newScheule)
        {
            var result = new CrudResult();

            if (string.IsNullOrEmpty(newScheule.BeginLocation))
            {
                result.AddError("Begin location is required.");
                return result;
            }
            if (string.IsNullOrEmpty(newScheule.EndLocation))
            {
                result.AddError("End location is required.");
                return result;
            }
            if (string.IsNullOrEmpty(newScheule.CustomerFullName))
            {
                result.AddError("Full name is required.");
                return result;
            }
            if (string.IsNullOrEmpty(newScheule.CustomerPhoneNumber))
            {
                result.AddError("Phone number is required.");
                return result;
            }
            if (string.IsNullOrEmpty(newScheule.NearLocation))
            {
                result.AddError("Near location is required.");
                return result;
            }
            var regex = new Regex("^(01[2689]|09|08[689])[0-9]{8}$");
            if (!regex.IsMatch(newScheule.CustomerPhoneNumber))
            {
                result.AddError("Wrong phone number format.");
            }

            return result;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if(Connections.Count > 0)
                Connections.RemoveByValue(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
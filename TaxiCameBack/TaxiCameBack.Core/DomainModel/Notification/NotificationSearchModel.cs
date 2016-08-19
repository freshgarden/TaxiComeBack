using System;

namespace TaxiCameBack.Core.DomainModel.Notification
{
    public class NotificationSearchModel
    {
        public string CustomerFullname { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string NearLocation { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}

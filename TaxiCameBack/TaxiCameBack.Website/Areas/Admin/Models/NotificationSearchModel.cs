using System;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class NotificationSearchModel
    {
        public string CustomerFullname { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string NearLocation { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool? Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}
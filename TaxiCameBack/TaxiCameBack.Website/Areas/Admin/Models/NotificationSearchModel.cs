using System;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class NotificationSearchModel
    {
        public string BeginLocation { get; set; }
        public string EndLocation { get; set; }
        public string NearLocation { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDateString { get; set; }
        public bool? Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}
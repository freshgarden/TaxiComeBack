using System;
using System.Web.UI.WebControls;

namespace TaxiCameBack.Website.Models
{
    public class CustomerRegisterCar
    {
        public Guid DriveId { get; set; }
        
        public string CustomerFullName { get; set; }
        
        public string CustomerPhoneNumber { get; set; }

        public Guid ScheduleId { get; set; }

        public string NearLocation { get; set; }
    }

    public class CustomerRegistrationNewScheule
    {
        public string BeginLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateDisplay { get; set; }
        public string StartDateHidden { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string NearLocation { get; set; }
        public string Message { get; set; }
    }
}
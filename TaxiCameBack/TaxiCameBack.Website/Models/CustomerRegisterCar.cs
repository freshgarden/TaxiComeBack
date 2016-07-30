namespace TaxiCameBack.Website.Models
{
    public class CustomerRegisterCar
    {
        public int DriveId { get; set; }
        
        public string CustomerFullName { get; set; }
        
        public string CustomerPhoneNumber { get; set; }

        public int ScheduleId { get; set; }

        public string NearLocation { get; set; }    }
}
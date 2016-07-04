namespace TaxiCameBack.DTO.Schedule
{
    public class ScheduleGeolocationDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

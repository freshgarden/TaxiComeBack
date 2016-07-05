using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.Schedule
{
    public class ScheduleGeolocation : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
//        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScheduleId { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
//        public Schedule Schedule { get; set; }
    }
}

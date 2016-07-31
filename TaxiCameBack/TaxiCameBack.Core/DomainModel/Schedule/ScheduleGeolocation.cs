using System;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Schedule
{
    public class ScheduleGeolocation : BaseEntity
    {
        public ScheduleGeolocation()
        {
            Id = GuidComb.GenerateComb();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
//        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ScheduleId { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
//        public Schedule Schedule { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.Schedule
{
    public class Schedule : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(300)]
        public string BeginLocation { get; set; }
        [Required]
        [MaxLength(300)]
        public string EndLocation { get; set; }
        [Required]
        public System.DateTime StartDate { get; set; }
//        public ScheduleGeolocation ScheduleGeolocation { get; set; }
        public virtual ICollection<ScheduleGeolocation> ScheduleGeolocations { get; set; }

    }
}

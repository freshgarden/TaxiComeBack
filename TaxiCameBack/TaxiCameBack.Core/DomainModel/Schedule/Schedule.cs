using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Schedule
{
    public class Schedule : BaseEntity
    {
        public Schedule()
        {
            Id = GuidComb.GenerateComb();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [MaxLength(300)]
        public string BeginLocation { get; set; }
        [Required]
        [MaxLength(300)]
        public string EndLocation { get; set; }
        [Required]
        public System.DateTime StartDate { get; set; }
        public virtual ICollection<ScheduleGeolocation> ScheduleGeolocations { get; set; }

        public virtual ICollection<Notification.Notification> Notifications { get; set; }
    }
}

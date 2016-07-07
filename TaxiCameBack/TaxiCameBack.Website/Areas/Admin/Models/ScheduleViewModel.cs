using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TaxiCameBack.Core.DomainModel.Schedule;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class ListScheduleViewModel
    {
        public IEnumerable<Schedule> Schedules { get; set; } 
    }
    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {
            ScheduleGeolocations = new List<ScheduleGeolocation>();
        }
        [HiddenInput]
        public int Id { get; set; }

        [DisplayName("Begin Location")]
        [Required]
        [StringLength(600)]
        public string BeginLocation { get; set; }

        [DisplayName("EndLocation")]
        [Required]
        [StringLength(600)]
        public string EndLocation { get; set; }

        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime StartDate { get; set; }

        [HiddenInput]
        [Required]
        public List<ScheduleGeolocation> ScheduleGeolocations { get; set; }
    }
}
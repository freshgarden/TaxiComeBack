using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class ListScheduleViewModel
    {
        public IEnumerable<Core.DomainModel.Schedule.Schedule> Schedules { get; set; } 
    }
    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {
            ScheduleGeolocations = new List<ScheduleGeolocation>();
        }
        [HiddenInput]
        public int Id { get; set; }

        [Display(ResourceType = typeof(CreateSchedule), Name = "lbl_begin_location")]
        [Required(ErrorMessageResourceType = typeof(CreateSchedule),ErrorMessageResourceName = "begin_location_required")]
        [StringLength(600, ErrorMessageResourceType = typeof(CreateSchedule), ErrorMessageResourceName = "begin_location_stringlength")]
        public string BeginLocation { get; set; }

        [Display(ResourceType = typeof(CreateSchedule), Name = "lbl_end_location")]
        [Required(ErrorMessageResourceType = typeof(CreateSchedule), ErrorMessageResourceName = "end_location_required")]
        [StringLength(600, ErrorMessageResourceType = typeof(CreateSchedule), ErrorMessageResourceName = "end_location_stringlength")]
        public string EndLocation { get; set; }

        [Display(ResourceType = typeof(CreateSchedule), Name = "lbl_start_date")]
        [Required(ErrorMessageResourceType = typeof(CreateSchedule), ErrorMessageResourceName = "start_date_required")]
        public DateTime StartDate { get; set; }

        [HiddenInput]
        [Required]
        public List<ScheduleGeolocation> ScheduleGeolocations { get; set; }
    }
}
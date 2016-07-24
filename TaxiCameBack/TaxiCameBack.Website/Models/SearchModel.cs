using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.Schedule;

namespace TaxiCameBack.Website.Models
{
    public class SearchModel
    {
        public double StartLocationLat { get; set; }
        public double StartLocationLng { get; set; }
        public double EndLocationLat { get; set; }
        public double EndLocationLng { get; set; }
        public string CarType { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ResultSearchModel
    {
        public string BeginLocation { get; set; }
        public string EndLocation { get; set; }
        public System.DateTime StartDate { get; set; }
        public string UserFullName { get; set; }
        public string UserAvatar { get; set; }
        public string UserGender { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserCarSitType { get; set; }
        public string UserCarNumber { get; set; }
        public string UserCarmakers { get; set; }
        public virtual ICollection<ScheduleGeolocation> ScheduleGeolocations { get; set; }
    }
}
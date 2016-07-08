using System;
using System.Collections.Generic;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.MapUtilities;

namespace TaxiCameBack.Services.Search
{
    public class SearchSchduleService : ISearchSchduleService
    {
        private readonly IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;

        public SearchSchduleService(
            IRepository<Core.DomainModel.Schedule.Schedule> scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
        
        public List<Core.DomainModel.Schedule.Schedule> Search(PointLatLng startPoint, PointLatLng endPoint)
        {
            if (!Util.IsValidatePoint(startPoint))
                throw new ArgumentOutOfRangeException();
            if (!Util.IsValidatePoint(endPoint))
                throw new ArgumentOutOfRangeException();

            var schedules = new List<Core.DomainModel.Schedule.Schedule>();
            var lstSchedules = _scheduleRepository.GetAll().ToList();
            var points = new List<PointLatLng>();
            foreach (var lstSchedule in lstSchedules)
            {
                points.AddRange(
                    lstSchedule.ScheduleGeolocations.Select(
                        schedule => new PointLatLng(schedule.Latitude, schedule.Longitude)));
                var distanceMeters = Util.GeoDistanceToPolyMtrs(points, startPoint);
                if (distanceMeters <= 5000)
                {
                    schedules.Add(lstSchedule);
                    points.Clear();
                }
                else
                {
                    points.Clear();
                }
            }
            return schedules;
        }
    }
}

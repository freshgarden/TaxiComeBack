﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.MapUtilities;

namespace TaxiCameBack.Services.Search
{
    public class SearchSchduleService : ISearchSchduleService
    {
        private readonly IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;
        private readonly IRepository<MembershipUser> _useRepository;

        public SearchSchduleService(
            IRepository<Core.DomainModel.Schedule.Schedule> scheduleRepository,
            IRepository<MembershipUser> useRepository )
        {
            _scheduleRepository = scheduleRepository;
            _useRepository = useRepository;
        }

        public List<Core.DomainModel.Schedule.Schedule> Search(PointLatLng startPoint, PointLatLng endPoint,
            string carType, DateTime startDate)
        {
            if (!Util.IsValidatePoint(startPoint))
                throw new ArgumentOutOfRangeException();
            if (!Util.IsValidatePoint(endPoint))
                throw new ArgumentOutOfRangeException();

            startDate = startDate >= DateTime.UtcNow.AddHours(7) ? startDate : DateTime.UtcNow.AddHours(7);

            var schedules = new List<Core.DomainModel.Schedule.Schedule>();
            var lstSchedules = _scheduleRepository.GetAll().Where(x => x.StartDate.Date == startDate.Date).ToList();
            var points = new List<PointLatLng>();
            foreach (var lstSchedule in lstSchedules)
            {
                if (lstSchedule.Notifications != null && lstSchedule.Notifications.Any(x => x.Received))
                    continue;
                points.AddRange(
                    lstSchedule.ScheduleGeolocations.Select(
                        schedule => new PointLatLng(schedule.Latitude, schedule.Longitude)));
                if (Util.GeoDistanceToPolyMtrs(points, startPoint) <= 5000
                    && Util.GeoDistanceToPolyMtrs(points, endPoint) <= 5000)
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

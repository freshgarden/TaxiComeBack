using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Castle.Core.Internal;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Logging;

namespace TaxiCameBack.Services.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;
        private readonly IRepository<ScheduleGeolocation> _scheduleGeolocationRepository;
        private readonly IRepository<Core.DomainModel.Notification.Notification> _notificationRepository;
        private readonly ILoggingService _loggingService;
        public ScheduleService(IRepository<Core.DomainModel.Schedule.Schedule> scheduleRepository, IRepository<ScheduleGeolocation> scheduleGeolocationRepository, IRepository<Core.DomainModel.Notification.Notification> notificationRepository, ILoggingService loggingService)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleGeolocationRepository = scheduleGeolocationRepository;
            _notificationRepository = notificationRepository;
            _loggingService = loggingService;
        }

        public List<Core.DomainModel.Schedule.Schedule> FindSchedulesByUser(Guid userId)
        {
            var schedules = _scheduleRepository.GetAll().Where(x => x.UserId == userId);
            var lstSchedules = schedules.ToList();
            return lstSchedules;
        }

        public Core.DomainModel.Schedule.Schedule FindScheduleById(Guid id)
        {
            return _scheduleRepository.GetById(id);
        }

        public ScheduleCreateResult DeleteSchedule(Guid scheduleId)
        {
            var result = new ScheduleCreateResult();
            var schedule = _scheduleRepository.GetById(scheduleId);
            var scheduleGeolocations = _scheduleGeolocationRepository.FindBy(x => x.ScheduleId == scheduleId).ToList();
            if (schedule == null)
            {
                result.AddError("Schedule not existed.");
                return result;
            }
            if (scheduleGeolocations.Count == 0)
            {
                result.AddError("Schedule Geolocaiton not existed.");
                return result;
            }
            try
            {
                foreach (var scheduleGeolocation in scheduleGeolocations)
                {
                    _scheduleGeolocationRepository.Delete(scheduleGeolocation);
                }
                _scheduleGeolocationRepository.UnitOfWork.Commit();

                _scheduleRepository.Delete(schedule);
                _scheduleRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                result.AddError(exception.Message);
            }
            return result;
        }

        public ScheduleCreateResult SaveScheduleInformation(Core.DomainModel.Schedule.Schedule schedule)
        {
            var result = new ScheduleCreateResult();

            Validate(result, schedule);
            if (result.Errors.Count > 0)
                return result;

            ValidateScheduleGeolocation(result, schedule);
            if (!result.Success)
                return result;

            try
            {
                _scheduleRepository.Insert(schedule);
                _scheduleRepository.UnitOfWork.Commit();
            }
            catch (DbEntityValidationException exception)
            {
                _scheduleRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }

            return result;
        }

        public ScheduleCreateResult UpdateScheduleInformation(Core.DomainModel.Schedule.Schedule schedule)
        {
            var result = new ScheduleCreateResult();

            Validate(result, schedule);
            if (result.Errors.Count > 0)
                return result;

            ValidateScheduleGeolocation(result, schedule);
            if (!result.Success)
                return result;

            try
            {
                // Delete all old schedules geolocation
                var oldScheduleGeolocations = _scheduleGeolocationRepository.GetAll().Where(x => x.ScheduleId == schedule.Id).ToList();
                foreach (var oldScheduleGeolocation in oldScheduleGeolocations)
                {
                    _scheduleGeolocationRepository.Delete(oldScheduleGeolocation);
                }

                // Insert all new schedules geolocation
                foreach (var scheduleGeolocation in schedule.ScheduleGeolocations)
                {
                    scheduleGeolocation.ScheduleId = schedule.Id;
                    _scheduleGeolocationRepository.Insert(scheduleGeolocation);
                }

                _scheduleGeolocationRepository.UnitOfWork.Commit();
                
                var oldSchedules = _scheduleRepository.GetById(schedule.Id);
                _scheduleRepository.Merge(oldSchedules, schedule);
                _scheduleRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _scheduleRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }

            return result;
        }

        public ScheduleCreateResult CancelSchedule(Core.DomainModel.Schedule.Schedule schedule)
        {
            var results = new ScheduleCreateResult();

            var oldSchedule = _scheduleRepository.GetById(schedule.Id);
            try
            {
                schedule.Notifications?.ForEach(x => x.IsCancel = true);
                schedule.IsCancel = true;
                _scheduleRepository.Merge(oldSchedule, schedule);
                _scheduleRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                results.AddError(exception.Message);
            }
            return results;
        }

        private void Validate(ScheduleCreateResult result, Core.DomainModel.Schedule.Schedule schedule)
        {
            schedule.BeginLocation = StringUtils.SafePlainText(schedule.BeginLocation);
            schedule.EndLocation = StringUtils.SafePlainText(schedule.EndLocation);

            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            if (schedule.ScheduleGeolocations == null)
                throw new ArgumentNullException("Can't load current schedule");

            if (string.IsNullOrEmpty(schedule.BeginLocation))
            {
                result.AddError("Null begin location");
                return;
            }

            if (string.IsNullOrEmpty(schedule.EndLocation))
            {
                result.AddError("Null end location");
                return;
            }

            if (schedule.StartDate.AddHours(7).Date < DateTime.UtcNow.AddHours(7).Date)
            {
                result.AddError("Start date must be equal or above today");
            }
        }

        private void ValidateScheduleGeolocation(ScheduleCreateResult result, Core.DomainModel.Schedule.Schedule schedule)
        {
            if (schedule.ScheduleGeolocations.ToList().Count == 0)
            {
                result.AddError("Schedule Geolocation can't be null.");
                return;
            }

            ICollection<ScheduleGeolocation> scheduleGeolocations = new List<ScheduleGeolocation>();
            foreach (var scheduleGeolocation in schedule.ScheduleGeolocations)
            {
                if (scheduleGeolocation == null)
                {
                    result.AddError("Schedule Geolocation can't be null");
                    return;
                }

                if (Math.Abs(scheduleGeolocation.Latitude) < 0.001 && Math.Abs(scheduleGeolocation.Longitude) < 0.001)
                {
                    result.AddError("Invalid Schedule Geolocation.");
                    return;
                }
                scheduleGeolocations.Add(scheduleGeolocation);
            }

            schedule.ScheduleGeolocations = scheduleGeolocations;
        }
    }
}

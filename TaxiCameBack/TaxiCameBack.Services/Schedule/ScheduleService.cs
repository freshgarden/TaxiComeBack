using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Logging;

namespace TaxiCameBack.Services.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;
        private IRepository<ScheduleGeolocation> _scheduleGeolocationRepository;
        private ILoggingService _loggingService;
        public ScheduleService(IRepository<Core.DomainModel.Schedule.Schedule> repository,
            IRepository<ScheduleGeolocation> scheduleGeolocationRepository,
            ILoggingService loggingService)
        {
            _scheduleRepository = repository;
            _scheduleGeolocationRepository = scheduleGeolocationRepository;
            _loggingService = loggingService;
        }

        public List<Core.DomainModel.Schedule.Schedule> FindSchedules()
        {
            var schedules = _scheduleRepository.GetAll().ToList();
            var lstSchedules = schedules.ToList();
            return lstSchedules;
        }

        public Core.DomainModel.Schedule.Schedule FindScheduleById(int id)
        {
            return _scheduleRepository.GetById(id);
        }

        public void DeleteSchedule(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public ScheduleCreateResult SaveScheduleInformation(Core.DomainModel.Schedule.Schedule schedule)
        {
            var result = new ScheduleCreateResult();

            Validate(result, schedule);
            if (result.Errors.Count > 0)
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

            var newScheduleGeolocations = new List<ScheduleGeolocation>();
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

                // Add all new schedules geolocation to schedule
                schedule.ScheduleGeolocations.Clear();
                foreach (var newScheduleGeolocation in newScheduleGeolocations)
                {
                    schedule.ScheduleGeolocations.Add(newScheduleGeolocation);
                }

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

            if (schedule.StartDate.Date < DateTime.Now.Date)
            {
                result.AddError("Start date must be equal or above today");
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Castle.Core.Internal;
using TaxiCameBack.Core;
using TaxiCameBack.Core.DomainModel.Schedule;

namespace TaxiCameBack.Services.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private IRepository<Core.DomainModel.Schedule.Schedule> _scheduleRepository;
        private IRepository<ScheduleGeolocation> _scheduleGeolocationRepository; 
        public ScheduleService(IRepository<Core.DomainModel.Schedule.Schedule> repository, IRepository<ScheduleGeolocation>  scheduleGeolocationRepository)
        {
            _scheduleRepository = repository;
            _scheduleGeolocationRepository = scheduleGeolocationRepository;
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
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            if (schedule.ScheduleGeolocations == null) 
                throw new ArgumentNullException("Can't load current schedule");

            var result = new ScheduleCreateResult();

            if (string.IsNullOrEmpty(schedule.BeginLocation))
            {
                result.AddError("Null begin location");
                return result;
            }

            if (string.IsNullOrEmpty(schedule.EndLocation))
            {
                result.AddError("Null end location");
                return result;
            }

            if (schedule.StartDate < DateTime.Today)
            {
                result.AddError("Start date must be equal or above today");
                return result;
            }

            try
            {
                _scheduleRepository.Insert(schedule);
                _scheduleRepository.UnitOfWork.Commit();
            }
            catch (DbEntityValidationException exception)
            {
                _scheduleRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
            }

            return result;
        }

        public void UpdateScheduleInformation(int id, Core.DomainModel.Schedule.Schedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}

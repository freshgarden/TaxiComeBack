using System.Collections.Generic;
using TaxiCameBack.DTO.Schedule;
using TaxiCameBack.Services.Common;

namespace TaxiCameBack.Services.Schedule
{
    public interface IScheduleService
    {
        /// <summary>
        /// Get all Schedules
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        List<Core.DomainModel.Schedule.Schedule> FindSchedules();

        /// <summary>
        /// Find Schedule by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Core.DomainModel.Schedule.Schedule FindScheduleById(int id);

        /// <summary>
        /// To Delete a schedule
        /// </summary>
        /// <param name="scheduleId"></param>
        void DeleteSchedule(int scheduleId);

        /// <summary>
        /// Add new schedule
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        ScheduleCreateResult SaveScheduleInformation(Core.DomainModel.Schedule.Schedule schedule);

        /// <summary>
        /// Update existing schedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="schedule"></param>
        void UpdateScheduleInformation(int id, Core.DomainModel.Schedule.Schedule schedule);
    }
}

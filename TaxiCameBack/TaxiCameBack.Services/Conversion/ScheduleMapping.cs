using System.Collections.Generic;
using TaxiCameBack.Core.DomainModel.Schedule;
using TaxiCameBack.DTO.Schedule;

namespace TaxiCameBack.Services.Conversion
{
    public class ScheduleMapping
    {
        public static ScheduleDto ScheduleToScheduleDto(Core.DomainModel.Schedule.Schedule schedule,
            List<ScheduleGeolocation> scheduleGeolocations)
        {
            var objScheduleDto = new ScheduleDto();
            objScheduleDto.Id = schedule.Id;
            objScheduleDto.BeginLocation = schedule.BeginLocation;
            objScheduleDto.EndLocation = schedule.EndLocation;
            objScheduleDto.StartDate = schedule.StartDate;
            objScheduleDto.ScheduleGeolocationDtos = new List<ScheduleGeolocationDto>();

            foreach (var scheduleGeolocation in scheduleGeolocations)
            {
                var objScheduleGeolocation = ScheduleGeolocationToScheduleGeolocationDto(scheduleGeolocation);
                objScheduleGeolocation.ScheduleId = schedule.Id;
                objScheduleDto.ScheduleGeolocationDtos.Add(objScheduleGeolocation);
            }

            return objScheduleDto;
        }

        public static ScheduleGeolocationDto ScheduleGeolocationToScheduleGeolocationDto(ScheduleGeolocation scheduleGeolocation)
        {
            ScheduleGeolocationDto scheduleGeolocationDto = new ScheduleGeolocationDto();
            if (scheduleGeolocation != null)
            {
                scheduleGeolocationDto.Id = scheduleGeolocation.Id;
                scheduleGeolocationDto.Latitude = scheduleGeolocation.Latitude;
                scheduleGeolocationDto.Longitude = scheduleGeolocation.Longitude;
            }
            return scheduleGeolocationDto;
        }
    }
}

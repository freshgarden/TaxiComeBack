using System;
using System.Collections;
using System.Collections.Generic;

namespace TaxiCameBack.DTO.Schedule
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BeginLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartDate { get; set; }
        public List<ScheduleGeolocationDto> ScheduleGeolocationDtos { get; set; }
    }
}

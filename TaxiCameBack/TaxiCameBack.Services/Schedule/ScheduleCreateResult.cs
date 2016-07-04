using System.Collections.Generic;

namespace TaxiCameBack.Services.Schedule
{
    public class ScheduleCreateResult
    {
        public ScheduleCreateResult()
        {
            Errors = new List<string>();
        }

        public bool Success => Errors.Count == 0;

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public IList<string> Errors { get; set; }
    }
}

using System.Collections.Generic;
using TaxiCameBack.Core.DomainModel.Logging;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class ListLogViewModel
    {
        public IList<LogEntry> LogFiles { get; set; }
    }
}
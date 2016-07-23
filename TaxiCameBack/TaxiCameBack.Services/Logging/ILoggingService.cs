using System;
using System.Collections.Generic;
using TaxiCameBack.Core.DomainModel.Logging;

namespace TaxiCameBack.Services.Logging
{
    public partial interface ILoggingService
    {
        void Error(string message);
        void Error(Exception ex);
        void Initialise(int maxLogSize);
        IList<LogEntry> ListLogFile();
        void Recycle();
        void ClearLogFiles();
    }
}

using System;
using System.Collections.Generic;
using TaxiCameBack.MapUtilities;

namespace TaxiCameBack.Services.Search
{
    public interface ISearchSchduleService
    {
        List<Core.DomainModel.Schedule.Schedule> Search(PointLatLng startPoint, PointLatLng endPoint, string carType, DateTime startDate);
    }
}

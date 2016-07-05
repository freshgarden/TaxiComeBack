using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.Schedule;

namespace TaxiCameBack.Data.Mapping.Schedule
{
    public class ScheduleGeolocationMapping : EntityTypeConfiguration<ScheduleGeolocation>
    {
        public ScheduleGeolocationMapping()
        {
            HasKey(s => s.Id);
            Property(s => s.ScheduleId).IsRequired();
            Property(s => s.Latitude).IsRequired();
            Property(s => s.Longitude).IsRequired();

//            HasRequired(s => s.Schedule)
//                .WithMany(s => s.ScheduleGeolocations)
//                .HasForeignKey(s => s.ScheduleId);
        }
    }
}

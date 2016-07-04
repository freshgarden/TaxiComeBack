using System.Data.Entity.ModelConfiguration;

namespace TaxiCameBack.Data.Mapping.Schedule
{
    public class ScheduleMapping : EntityTypeConfiguration<Core.DomainModel.Schedule.Schedule>
    {
        public ScheduleMapping()
        {
            HasKey(c => c.Id);
            Property(s => s.BeginLocation).HasMaxLength(300).IsRequired();
            Property(s => s.EndLocation).HasMaxLength(300).IsRequired();
            Property(s => s.StartDate).IsRequired();
        }
    }
}

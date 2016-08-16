using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.Notification;

namespace TaxiCameBack.Data.Mapping.Notification
{
    public class NotificationExtendMapping : EntityTypeConfiguration<NotificationExtend>
    {
        public NotificationExtendMapping()
        {
            HasKey(x => x.Id);
            Property(s => s.BeginLocation).HasMaxLength(300).IsRequired();
            Property(s => s.EndLocation).HasMaxLength(300).IsRequired();
            Property(s => s.StartDate).IsRequired();
        }
    }
}

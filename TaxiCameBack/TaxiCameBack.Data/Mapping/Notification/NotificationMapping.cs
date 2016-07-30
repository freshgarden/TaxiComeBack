using System.Data.Entity.ModelConfiguration;

namespace TaxiCameBack.Data.Mapping.Notification
{
    public class NotificationMapping : EntityTypeConfiguration<Core.DomainModel.Notification.Notification>
    {
        public NotificationMapping()
        {
            HasKey(c => c.Id);
            Property(c => c.Message).IsRequired();
            Property(c => c.CreateDate).IsOptional();
        }
    }
}

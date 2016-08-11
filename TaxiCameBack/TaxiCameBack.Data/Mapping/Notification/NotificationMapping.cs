using System.Data.Entity.ModelConfiguration;

namespace TaxiCameBack.Data.Mapping.Notification
{
    public class NotificationMapping : EntityTypeConfiguration<Core.DomainModel.Notification.Notification>
    {
        public NotificationMapping()
        {
            HasKey(c => c.Id);
            Property(c => c.CustomerFullname).IsRequired();
            Property(c => c.CustomerPhoneNumber).IsRequired();
            Property(c => c.NearLocation).IsRequired();
            Property(c => c.CreateDate).IsOptional();
            Property(c => c.ReceivedDate).IsOptional();
        }
    }
}

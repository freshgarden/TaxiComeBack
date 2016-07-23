using System.Data.Entity.ModelConfiguration;

namespace TaxiCameBack.Data.Mapping.Settings
{
    public class SettingsMapping : EntityTypeConfiguration<Core.DomainModel.Settings.Settings>
    {
        public SettingsMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.SiteName).IsOptional().HasMaxLength(500);
            Property(x => x.SiteUrl).IsOptional().HasMaxLength(500);
            Property(x => x.AdminEmailAddress).IsOptional().HasMaxLength(100);
            Property(x => x.NotificationReplyEmail).IsOptional().HasMaxLength(100);
            Property(x => x.SMTP).IsOptional().HasMaxLength(100);
            Property(x => x.SMTPUsername).IsOptional().HasMaxLength(100);
            Property(x => x.SMTPPort).IsOptional().HasMaxLength(10);
            Property(x => x.SMTPEnableSSL).IsOptional();
            Property(x => x.SMTPPassword).IsOptional().HasMaxLength(100);
        }
    }
}

using System;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Settings
{
    public class Settings : BaseEntity
    {
        public Settings()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string AdminEmailAddress { get; set; }
        public string NotificationReplyEmail { get; set; }
        public string SMTP { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPPort { get; set; }
        public bool? SMTPEnableSSL { get; set; }
        public virtual MembershipRole NewMemberStartingRole { get; set; }
    }
}

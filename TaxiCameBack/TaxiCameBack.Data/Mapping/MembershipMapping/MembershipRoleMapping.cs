using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.Membership;

namespace TaxiCameBack.Data.Mapping.MembershipMapping
{
    public class MembershipRoleMapping : EntityTypeConfiguration<MembershipRole>
    {
        public MembershipRoleMapping()
        {
            HasKey(cr => cr.RoleId);
            Property(cr => cr.RoleName).IsRequired().HasMaxLength(255);
        }
    }
}

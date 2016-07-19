using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.Membership;

namespace TaxiCameBack.Data.Mapping.MembershipMapping
{
    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            HasKey(c => c.UserId);
            Property(u => u.Email).IsRequired().HasMaxLength(1000);
            
            HasMany(c => c.Roles)
                .WithMany(r => r.Users)
                .Map(
                    m =>
                    {
                        m.ToTable("UserRoles");
                        m.MapLeftKey("UserId");
                        m.MapRightKey("RoleId");
                    }
                );

            HasMany(c => c.Schedules)
                .WithRequired()
                .HasForeignKey(s => s.UserId);
        }
    }
}

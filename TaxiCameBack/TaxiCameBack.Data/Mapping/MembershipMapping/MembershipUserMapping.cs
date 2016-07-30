using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.Membership;

namespace TaxiCameBack.Data.Mapping.MembershipMapping
{
    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            HasKey(c => c.UserId);
            Property(u => u.Email).IsRequired().HasMaxLength(256);
            
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

            HasMany(c => c.Notifications)
                .WithRequired()
                .HasForeignKey(s => s.UserId);

            Property(x => x.Password).IsRequired().HasMaxLength(128);
            Property(x => x.PasswordSalt).IsOptional().HasMaxLength(128);
            Property(x => x.Active).IsRequired();
            Property(x => x.IsLockedOut).IsRequired();
            Property(x => x.FullName).IsRequired();
            Property(x => x.Address).IsOptional();
            Property(x => x.Gender).IsOptional();
            Property(x => x.PhoneNumber).IsRequired();
            Property(x => x.DateOfBirth).IsOptional();
            Property(x => x.CreatedOnUtc).IsRequired();
            Property(x => x.LastLoginDateUtc).IsRequired();
            Property(x => x.CarNumber).IsOptional();
            Property(x => x.LastLockoutDate).IsOptional();
            Property(x => x.FailedPasswordAttemptCount).IsOptional();
            Property(x => x.Carmakers).IsOptional();
            Property(x => x.PasswordResetToken).HasMaxLength(150).IsOptional();
            Property(x => x.PasswordResetTokenCreatedAt).IsOptional();
            Property(x => x.Avatar).IsOptional().HasMaxLength(500);
        }
    }
}

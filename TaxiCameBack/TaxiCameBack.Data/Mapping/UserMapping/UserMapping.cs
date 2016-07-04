using System.Data.Entity.ModelConfiguration;

namespace TaxiCameBack.Data.Mapping.UserMapping
{
    public class UserMapping : EntityTypeConfiguration<Core.DomainModel.User.User>
    {
        public UserMapping()
        {
            HasKey(c => c.Id);
            Property(u => u.Username).IsRequired().HasMaxLength(1000);
            Property(u => u.Email).IsRequired().HasMaxLength(1000);
            
            HasMany(c => c.UserRoles)
                .WithMany()
                .Map(m => m.ToTable("User_UserRole_Mapping"));
        }
    }
}

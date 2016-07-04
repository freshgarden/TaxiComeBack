using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.ProfileAggregate;

namespace TaxiCameBack.Data.EntityConfiguration
{
    class ProfileConfiguration : EntityTypeConfiguration<Profile>
    {
        public ProfileConfiguration()
        {
            this.HasKey(p => p.ProfileId);
            this.Property(p => p.FirstName).HasMaxLength(50).IsRequired();
            this.Property(p => p.LastName).HasMaxLength(50).IsRequired();
            this.Property(p => p.Email).HasMaxLength(50).IsRequired();

            //configure table map
            this.ToTable("Profile");
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.PhoneAggregate;

namespace TaxiCameBack.Data.EntityConfiguration
{
    class PhoneConfiguration : EntityTypeConfiguration<Phone>
    {
        public PhoneConfiguration()
        {
            this.HasKey(p => p.PhoneId);
            this.Property(p => p.Number).HasMaxLength(25).IsRequired();

            //configure table map
            this.ToTable("Phone");
        }
    }
}

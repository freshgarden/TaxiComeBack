using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.AddressAggregate;

namespace TaxiCameBack.Data.EntityConfiguration
{
    class AddressTypeConfiguration : EntityTypeConfiguration<AddressType>
    {
        public AddressTypeConfiguration()
        {
            this.HasKey(at => at.AddressTypeId);
            this.Property(at => at.Name).HasMaxLength(50).IsRequired();

            //configure table map
            this.ToTable("AddressType");
        }
    }
}

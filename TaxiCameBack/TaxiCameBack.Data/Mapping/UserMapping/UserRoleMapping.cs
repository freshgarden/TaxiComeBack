using System.Data.Entity.ModelConfiguration;
using TaxiCameBack.Core.DomainModel.User;

namespace TaxiCameBack.Data.Mapping.UserMapping
{
    public class UserRoleMapping : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMapping()
        {
            HasKey(cr => cr.Id);
            Property(cr => cr.Name).IsRequired().HasMaxLength(255);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.User
{
    public class UserRole : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active
        /// </summary>
        public bool Active { get; set; }
    }
}

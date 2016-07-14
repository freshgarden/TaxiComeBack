using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.Membership
{
    public class MembershipRole : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }
        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public string RoleName { get; set; }
        public virtual IList<MembershipUser> Users { get; set; }
    }
}

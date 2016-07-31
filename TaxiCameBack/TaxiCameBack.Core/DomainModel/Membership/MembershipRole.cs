using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Membership
{
    public partial class MembershipRole : BaseEntity
    {
        public MembershipRole()
        {
            RoleId = GuidComb.GenerateComb();
        }
        [Key]
        public Guid RoleId { get; set; }
        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public string RoleName { get; set; }
        public virtual IList<MembershipUser> Users { get; set; }
        public virtual Settings.Settings Settings { get; set; }
    }
}

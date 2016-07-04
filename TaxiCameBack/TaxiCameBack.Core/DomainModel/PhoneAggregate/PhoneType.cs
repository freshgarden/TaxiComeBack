using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.ProfilePhoneAggregate;

namespace TaxiCameBack.Core.DomainModel.PhoneAggregate
{
    public class PhoneType : BaseEntity
    {
        #region Constructor

        public PhoneType()
        {
            this.ProfilePhones = new HashSet<ProfilePhone>();
        }

        #endregion Constructor

        #region Property

        [Key]
        public int PhoneTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProfilePhone> ProfilePhones { get; set; }

        #endregion Property

    }
}

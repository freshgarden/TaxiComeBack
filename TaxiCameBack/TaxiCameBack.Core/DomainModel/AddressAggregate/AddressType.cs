using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.ProfileAddressAggregate;

namespace TaxiCameBack.Core.DomainModel.AddressAggregate
{
    public class AddressType : BaseEntity
    {
        
        #region Constructor

        public AddressType()
        {
            this.ProfileAddresses = new HashSet<ProfileAddress>();
        }

        #endregion Constructor

        #region Property

        [Key]
        public int AddressTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProfileAddress> ProfileAddresses { get; set; }

        #endregion Property

    }
}

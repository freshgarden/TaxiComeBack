using System;
using TaxiCameBack.Core.DomainModel.AddressAggregate;
using TaxiCameBack.Core.DomainModel.ProfileAggregate;

namespace TaxiCameBack.Core.DomainModel.ProfileAddressAggregate
{
    /// <summary>
    /// This is the factory for Phone creation
    /// </summary>
    public static class ProfileAddressFactory
    {
        public static ProfileAddress ProfileAddress(Profile profile, Address address, AddressType addressType, string createdBy, DateTime created, string updatedBy, DateTime updated)
        {
            ProfileAddress objProfileAddress = new ProfileAddress();

            //Set values for Address
            objProfileAddress.Created = created;
            objProfileAddress.CreatedBy = createdBy;
            objProfileAddress.Updated = updated;
            objProfileAddress.UpdatedBy = updatedBy;

            //Associate Profile for this Profile Phone
            objProfileAddress.ProfileId = profile.ProfileId;

            //Associate Address for this Profile Phone
            objProfileAddress.AddressId = address.AddressId;

            //Associate AddressTye for this Profile Phone
            objProfileAddress.AddressTypeId = addressType.AddressTypeId;

            return objProfileAddress;
        }

    }
}

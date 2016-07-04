using System.Collections.Generic;

namespace TaxiCameBack.DTO.ProfileModule
{
    public class ContactForm
    {
        public List<AddressTypeDTO> lstAddressTypeDTO { get; set; }
        public List<PhoneTypeDTO> lstPhoneTypeDTO { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.ProfileAddressAggregate;
using TaxiCameBack.Core.DomainModel.ProfilePhoneAggregate;

namespace TaxiCameBack.Core.DomainModel.ProfileAggregate
{
    public partial class Profile : BaseEntity
    {
       #region Constructor

        public Profile()
        {
            this.ProfileAddresses = new HashSet<ProfileAddress>();
            this.ProfilePhones = new HashSet<ProfilePhone>();
        }

        #endregion Constructor

        #region Properties
        [Key]
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<ProfileAddress> ProfileAddresses { get; set; }
        public virtual ICollection<ProfilePhone> ProfilePhones { get; set; }

        #endregion Properties

        #region IValidatableObject Members

        /// <summary>
        /// This will validate entity for all  the conditions
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            var validationResults = new List<ValidationResult>();
//
//            //-->Check FirstName property
//            if (String.IsNullOrWhiteSpace(this.FirstName))
//            {
//                validationResults.Add(new ValidationResult(Messages.validation_ProfileFirstNameCannotBeNull,
//                                                           new string[] { "AddressLine1" }));
//            }
//
//            //-->Check LastName property
//            if (String.IsNullOrWhiteSpace(this.LastName))
//            {
//                validationResults.Add(new ValidationResult(Messages.validation_ProfileLastNameCannotBeBull,
//                                                           new string[] { "AddressLine2" }));
//            }
//
//            //-->Check Email property
//            if (String.IsNullOrWhiteSpace(this.Email))
//            {
//                validationResults.Add(new ValidationResult(Messages.validation_ProfileEmailCannotBeBull,
//                                                           new string[] { "ZipCode" }));
//            }
//
//            return validationResults;
//        }

        #endregion

    }
}

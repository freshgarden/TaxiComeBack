using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Website.App_LocalResources;
using System.Web;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_fullname")]
        [StringLength(50, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "fullname_length")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_phone")]
        [RegularExpression(@"^(0\d{9,10})$", ErrorMessage = "Invalid phone.")]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_email")]
        [StringLength(50, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "email_length")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_password")]
        [StringLength(50, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "password_length")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_re_password")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "re_password_match")]
        public string RetypePassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_agree_term")]
        public bool AgreeTerm { get; set; }

        [DisplayName("Upload New Avatar")]
        public HttpPostedFileBase File { get; set; }

        public string Avatar { get; set; }

        [DisplayName("Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Day")]
        [Required]
        public int Day { get; set; }

        [DisplayName("Month")]
        [Required]
        public int Month { get; set; }

        [DisplayName("Year")]
        [Required]
        public int Year { get; set; }

        [DisplayName("Address")]
        [Required]
        public string Address { get; set; }

        [DisplayName("Gender")]
        [Required]
        public string Gender { get; set; }
        
        [DisplayName("Car Sit Type")]
        [Required]
        public string CarSitType { get; set; }

        [DisplayName("Car Number")]
        [Required]
        public string CarNumber { get; set; }

        [DisplayName("Carmakers")]
        [Required]
        public string Carmakers { get; set; }
    }
}
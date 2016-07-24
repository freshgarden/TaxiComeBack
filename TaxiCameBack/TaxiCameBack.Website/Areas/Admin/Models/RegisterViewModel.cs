using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Website.App_LocalResources;
using System.Web;
using TaxiCameBack.Website.Application.Attributes;

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

        [MustBeTrue(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_agree_term")]
        public bool AgreeTerm { get; set; }
    }
}
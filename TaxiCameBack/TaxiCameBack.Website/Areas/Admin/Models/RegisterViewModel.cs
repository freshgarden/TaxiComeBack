using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TaxiCameBack.Website.App_LocalResources;
using TaxiCameBack.Website.Application.Attributes;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_fullname")]
        [StringLength(20, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "fullname_length")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_phone")]
        [RegularExpression(@"^(0\d{9,10})$", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "valid_phone")]
        public string Phone { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "valid_email")]
        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_email")]
        [Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "uniq_email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_password")]
        [StringLength(20, MinimumLength = 8, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "password_length")]
        [RegularExpression(@"^\S*$", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "valid_password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_re_password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "re_password_match")]
        public string RetypePassword { get; set; }

        [MustBeTrue(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_agree_term")]
        public bool AgreeTerm { get; set; }
    }
}
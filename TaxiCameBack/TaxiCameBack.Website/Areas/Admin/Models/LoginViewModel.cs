using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        [Required(ErrorMessageResourceType = typeof(Login),ErrorMessageResourceName = "rqr_username")]
        [StringLength(50,ErrorMessageResourceType = typeof(Login), ErrorMessageResourceName = "username_length")]
        public string UserName { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Login), ErrorMessageResourceName = "rqr_password")]
        [StringLength(50, ErrorMessageResourceType = typeof(Login), ErrorMessageResourceName = "password_length")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string Message { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string Message { get; set; }
    }
}
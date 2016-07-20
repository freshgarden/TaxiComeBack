using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class MemberViewModels
    {
        public class ForgotPasswordViewModel
        {
            [EmailAddress]
            [Required]
            public string EmailAddress { get; set; }
        }

        public class ResetPasswordViewModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Token { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
            public string ConfirmPassword { get; set; }

        }
    }
}
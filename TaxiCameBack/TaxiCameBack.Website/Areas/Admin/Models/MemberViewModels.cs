using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class MemberViewModels
    {
        public class ForgotPasswordViewModel
        {
            [EmailAddress(ErrorMessageResourceType = typeof(ForgotPassword),ErrorMessageResourceName = "email_type", ErrorMessage = null)]
            [Required(ErrorMessageResourceType = typeof(ForgotPassword),ErrorMessageResourceName = "rqr_email")]
            public string EmailAddress { get; set; }

            public string Message { get; set; }
        }

        public class SelfMemberEditViewModel
        {
            [Required]
            public Guid Id { get; set; }
          
            [Display(ResourceType = typeof(UserProfile),Name = "email_address")]
            [EmailAddress]
            [Required]
            public string Email { get; set; }
            
            [Display(ResourceType = typeof(UserProfile), Name = "upload_avatar")]
            public HttpPostedFileBase File { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "full_name")]
            [Required]
            public string FullName { get; set; }

            public string Avatar { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "date_of_birth")]
            public DateTime? DateOfBirth { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "day")]
            public int Day { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "month")]
            public int Month { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "year")]
            public int Year { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "address")]
            public string Address { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "gender")]
            public string Gender { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "phone")]
            [Required]
            [RegularExpression(@"^(0\d{9,10})$", ErrorMessageResourceType = typeof(UserProfile),ErrorMessageResourceName = "invalid_phone_number")]
            public string PhoneNumber { get; set; }
            
            [Display(ResourceType = typeof(UserProfile), Name = "plate_number")]
            public string CarNumber { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "car_branch")]
            public string Carmakers { get; set; }

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
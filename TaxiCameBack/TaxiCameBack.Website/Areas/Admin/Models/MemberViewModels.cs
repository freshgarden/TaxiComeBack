using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class MemberViewModels
    {
        public class ForgotPasswordViewModel
        {
            [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceType = typeof(ForgotPassword), ErrorMessageResourceName = "email_type")]            
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
            
            [ImageValidate("jpeg|png|jpg|gif", 1 * 1024 * 1024)]
            [Display(ResourceType = typeof(UserProfile), Name = "upload_avatar")]
            public HttpPostedFileBase File { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "full_name")]
            [Required(ErrorMessageResourceType = typeof(UserProfile), ErrorMessageResourceName = "rqr_fullname")]
            [StringLength(20, ErrorMessageResourceType = typeof(UserProfile), ErrorMessageResourceName = "fullname_length")]
            public string FullName { get; set; }

            public string Avatar { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "date_of_birth")]
            public DateTime? DateOfBirth { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "day")]
            public int? Day { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "month")]
            public int? Month { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "year")]
            public int? Year { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "address")]
            public string Address { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "gender")]
            public string Gender { get; set; }

            [Display(ResourceType = typeof(UserProfile), Name = "phone")]
            [Required(ErrorMessageResourceType = typeof(UserProfile), ErrorMessageResourceName = "rqr_phone")]
            [RegularExpression(@"^(0\d{9,10})$", ErrorMessageResourceType = typeof(UserProfile), ErrorMessageResourceName = "valid_phone")]
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

            [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_password")]
            [StringLength(20, MinimumLength = 8, ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "password_length")]
            [RegularExpression(@"^\S*$", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "valid_password")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required(ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "rqr_re_password")]
            [DataType(DataType.Password)]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Register), ErrorMessageResourceName = "re_password_match")]
            public string ConfirmPassword { get; set; }

        }
    }
}
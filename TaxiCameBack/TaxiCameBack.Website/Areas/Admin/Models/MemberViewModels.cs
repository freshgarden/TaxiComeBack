using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class MemberViewModels
    {
        public class ForgotPasswordViewModel
        {
            [EmailAddress(ErrorMessageResourceType = typeof(ForgotPassword),ErrorMessageResourceName = "email_type")]
            [Required(ErrorMessageResourceType = typeof(ForgotPassword),ErrorMessageResourceName = "rqr_email")]
            public string EmailAddress { get; set; }

            public string Message { get; set; }
        }

        public class SelfMemberEditViewModel
        {
            [Required]
            public int Id { get; set; }
          
            [DisplayName("Email Address")]
            [EmailAddress]
            [Required]
            public string Email { get; set; }
            
            [DisplayName("Upload New Avatar")]
            public HttpPostedFileBase File { get; set; }

            [DisplayName("Full name")]
            [Required]
            public string FullName { get; set; }

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

            [DisplayName("Phone Number")]
            [Required]
            [RegularExpression(@"^(0\d{9,10})$", ErrorMessage = "Invalid phone.")]
            public string PhoneNumber { get; set; }

            [DisplayName("Car Number")]
            [Required]
            public string CarNumber { get; set; }

            [DisplayName("Carmakers")]
            [Required]
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
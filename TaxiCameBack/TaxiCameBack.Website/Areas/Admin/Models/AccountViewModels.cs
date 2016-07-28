using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Website.App_LocalResources;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class SingleMemberListViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Locked Out")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Roles")]
        public string[] Roles { get; set; }
    }

    public class MemberListViewModel
    {
        [Required]
        [Display(Name = "Users")]
        public IList<SingleMemberListViewModel> Users { get; set; }

//        [Required]
//        [Display(Name = "Roles")]
//        public IList<MembershipRole> AllRoles { get; set; }

        public int Id { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }

    }

    public class MemberEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(ResourceType = typeof(ApproveUser),Name = "email_address")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "user_approved",ResourceType = typeof(ApproveUser))]
        public bool IsApproved { get; set; }
        
        [Display(Name = "user_locked",ResourceType = typeof(ApproveUser))]
        public bool IsLockedOut { get; set; }

        [Display(Name = "roles",ResourceType = typeof (ApproveUser))]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.DomainModel.Membership;

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

        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "User is Approved")]
        public bool IsApproved { get; set; }
        
        [Display(Name = "User is Locked Out")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Roles")]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }

    }
}
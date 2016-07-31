using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class EditSettingsViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Site Name")]
        [Required]
        [StringLength(200)]
        public string SiteName { get; set; }

        [DisplayName("Site Url")]
        [Required]
        [StringLength(200)]
        public string SiteUrl { get; set; }

        [EmailAddress]
        [DisplayName("Admin Email Address")]
        public string AdminEmailAddress { get; set; }

        [DisplayName("Notification Reply Email Address")]
        [AllowHtml] // We have to put this to allow this type of reply address MVCForum <noreply@mvcforum.com>
        public string NotificationReplyEmail { get; set; }

        [DisplayName("SMTP Server")]
        public string SMTP { get; set; }

        [DisplayName("SMTP Server Username")]
        public string SMTPUsername { get; set; }

        [DisplayName("SMTP Server Password")]
        public string SMTPPassword { get; set; }

        [DisplayName("Optional: SMTP Port")]
        public int? SMTPPort { get; set; }

        [DisplayName("SMTP SSL - Enable SSL for sending via gmail etc...")]
        public bool SMTPEnableSSL { get; set; }
    }
}
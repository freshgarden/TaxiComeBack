using System;
using System.Linq;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.Settings;

namespace TaxiCameBack.Website.Areas.Admin.Models.Mapping
{
    public class ViewModelMapping
    {
        public static SingleMemberListViewModel UserToSingleMemberListViewModel(MembershipUser user)
        {
            var viewModel = new SingleMemberListViewModel
            {
                IsApproved = user.Active,
                Id = user.UserId,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserEmail = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber
            };
            return viewModel;
        }

        public static MemberEditViewModel UserToMemberEditViewModel(MembershipUser user)
        {
            var viewModel = new MemberEditViewModel
            {
                IsApproved = user.Active,
                Id = user.UserId,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                Email = user.Email,
            };
            return viewModel;
        }

        #region Settings
        public static Settings SettingsViewModelToSettings(EditSettingsViewModel settingsViewModel, Settings existingSettings)
        {
            //NOTE: The only reason some properties are commented out, are because those items were
            //      moved to their own page when the admin was refactored.

            existingSettings.Id = settingsViewModel.Id;
            existingSettings.SiteName = settingsViewModel.SiteName;
            existingSettings.SiteUrl = settingsViewModel.SiteUrl;
            existingSettings.AdminEmailAddress = settingsViewModel.AdminEmailAddress;
            existingSettings.NotificationReplyEmail = settingsViewModel.NotificationReplyEmail;
            existingSettings.SMTP = settingsViewModel.SMTP;
            existingSettings.SMTPUsername = settingsViewModel.SMTPUsername;
            existingSettings.SMTPPassword = settingsViewModel.SMTPPassword;
            existingSettings.SMTPPort = settingsViewModel.SMTPPort.ToString();
            existingSettings.SMTPEnableSSL = settingsViewModel.SMTPEnableSSL;
            
            return existingSettings;
        }

        public static EditSettingsViewModel SettingsToSettingsViewModel(Settings currentSettings)
        {
            var settingViewModel = new EditSettingsViewModel
            {
                Id = currentSettings.Id,
                SiteName = currentSettings.SiteName,
                SiteUrl = currentSettings.SiteUrl,
                AdminEmailAddress = currentSettings.AdminEmailAddress,
                NotificationReplyEmail = currentSettings.NotificationReplyEmail,
                SMTP = currentSettings.SMTP,
                SMTPUsername = currentSettings.SMTPUsername,
                SMTPPassword = currentSettings.SMTPPassword,
                
                SMTPPort = string.IsNullOrEmpty(currentSettings.SMTPPort) ? null : (int?)(Convert.ToInt32(currentSettings.SMTPPort)),
                
                SMTPEnableSSL = currentSettings.SMTPEnableSSL ?? false,
                
            };

            return settingViewModel;
        }
        #endregion

        public static Core.DomainModel.Notification.NotificationSearchModel SearchModelToDomainSearchModel(
            NotificationSearchModel searchModel)
        {
            if (searchModel == null)
                return null;
            return new Core.DomainModel.Notification.NotificationSearchModel
            {
                BeginLocation = searchModel.BeginLocation,
                EndLocation = searchModel.EndLocation,
                NearLocation = searchModel.NearLocation,
                Received = searchModel.Received,
                StartDate = searchModel.StartDate,
                CreateDate = searchModel.CreateDate,
                ReceivedDate = searchModel.ReceivedDate
            };
        }
    }
}
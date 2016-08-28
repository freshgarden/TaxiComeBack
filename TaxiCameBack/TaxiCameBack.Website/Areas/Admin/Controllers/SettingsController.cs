using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Email;
using TaxiCameBack.Services.Email;
using TaxiCameBack.Services.Logging;
using TaxiCameBack.Services.Settings;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Areas.Admin.Models;
using TaxiCameBack.Website.Areas.Admin.Models.Mapping;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
    public class SettingsController : BaseController
    {
        protected readonly ISettingsService _settingsService;
        protected readonly ILoggingService LoggingService;
        private readonly IEmailService _emailService;
        
        public SettingsController(
            ISettingsService settingsService, 
            ILoggingService loggingService,
            IEmailService emailService)
        {
            _settingsService = settingsService;
            LoggingService = loggingService;
            _emailService = emailService;
        }
        
        public ActionResult Index()
        {
            var currentSettings = _settingsService.GetSettings();
            var settingViewModel = ViewModelMapping.SettingsToSettingsViewModel(currentSettings);
            return View(settingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EditSettingsViewModel settingsViewModel)
        {
            if (ModelState.IsValid)
            {
                var existingSettings = _settingsService.GetSettings();
                var updatedSettings = ViewModelMapping.SettingsViewModelToSettings(settingsViewModel, existingSettings);
                var result = _settingsService.Update(existingSettings, updatedSettings);
                if (result.Success)
                {
                    // All good clear cache and get reliant lists
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = App_LocalResources.SettingPage.update_setting_success,
                        MessageType = GenericMessages.success
                    };
                }
                else
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = App_LocalResources.SettingPage.update_setting_failed,
                        MessageType = GenericMessages.danger
                    };
                }
            }
            return View(settingsViewModel);
        }

        public ActionResult TestEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendTestEmail()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<p>{0}</p>",
                string.Concat("This is a test email from ", _settingsService.GetSettings().SiteName));
            var email = new Email
            {
                EmailTo = _settingsService.GetSettings().AdminEmailAddress,
                NameTo = "Email Test Admin",
                Subject = string.Concat("Email Test From ", _settingsService.GetSettings().SiteName)
            };
            email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());


            var message = new GenericMessageViewModel();

            try
            {
                await Task.Run(() => _emailService.SendMail(email));
                message.Message = App_LocalResources.SettingPage.test_email_success;
                message.MessageType = GenericMessages.success;
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                message.Message = App_LocalResources.SettingPage.test_email_failed;
                message.MessageType = GenericMessages.danger;
            }

            TempData[AppConstants.MessageViewBagName] = message;

            return RedirectToAction("Index");
        }

    }
}
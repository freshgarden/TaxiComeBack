using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Services.Logging;
using TaxiCameBack.Services.Settings;

namespace TaxiCameBack.Services.Email
{
    public class EmailService : IEmailService
    {
        public List<Core.DomainModel.Email.Email> Emails { get; private set; }
        private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;

        public EmailService(ISettingsService settingsService, ILoggingService loggingService)
        {
            _settingsService = settingsService;
            _loggingService = loggingService;
        }

        public EmailService()
        {
            Emails = new List<Core.DomainModel.Email.Email>();
        }


        public void SendMail(Core.DomainModel.Email.Email email)
        {
            SendMail(new List<Core.DomainModel.Email.Email> { email });
        }

        public void SendMail(List<Core.DomainModel.Email.Email> email)
        {
            Emails.AddRange(email);
            ProcessMail();
        }

        public string EmailTemplate(string to, string content)
        {
            using (var sr = File.OpenText(HostingEnvironment.MapPath(@"~/Content/Emails/EmailNotification.htm")))
            {
                var settings = _settingsService.GetSettings();
                var sb = sr.ReadToEnd();
                sr.Close();
                sb = sb.Replace("#CONTENT#", content);
                sb = sb.Replace("#SITENAME#", settings.SiteName);
                sb = sb.Replace("#SITEURL#", settings.SiteUrl);
                if (!string.IsNullOrEmpty(to))
                {
                    to = $"<p>{to},</p>";
                    sb = sb.Replace("#TO#", to);
                }

                return sb;
            }
        }

        private async void ProcessMail()
        {
            try
            {
                if (Emails != null && Emails.Any())
                {
                    // Get the mails settings
                    var settings = _settingsService.GetSettings();
                    var smtp = settings.SMTP;
                    var smtpUsername = settings.SMTPUsername;
                    var smtpPassword = settings.SMTPPassword;
                    var smtpPort = settings.SMTPPort;
                    var smtpEnableSsl = settings.SMTPEnableSSL;
                    var fromEmail = settings.NotificationReplyEmail;

                    // If no SMTP settings then log it
                    if (string.IsNullOrEmpty(smtp))
                    {
                        // Not logging as it makes the log file massive
                        //_loggingService.Error("There are no SMTP details in the settings, unable to send emails");
                        return;
                    }

                    // Set up the SMTP Client object and settings
                    var mySmtpClient = new SmtpClient(smtp);
                    if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        mySmtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    }

                    if (smtpEnableSsl != null)
                    {
                        mySmtpClient.EnableSsl = (bool)smtpEnableSsl;
                    }

                    if (!string.IsNullOrEmpty(smtpPort))
                    {
                        mySmtpClient.Port = Convert.ToInt32(smtpPort);
                    }

                    // List to store the emails to delete after they are sent
                    var emailsToDelete = new List<Core.DomainModel.Email.Email>();

                    // Loop through email email create a mailmessage and send it
                    foreach (var message in Emails)
                    {
                        var msg = new MailMessage
                        {
                            IsBodyHtml = true,
                            Body = message.Body,
                            From = new MailAddress(fromEmail),
                            Subject = message.Subject
                        };
                        msg.To.Add(message.EmailTo);
                        await mySmtpClient.SendMailAsync(msg);

                        emailsToDelete.Add(message);
                    }
                }
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
            }
        }
    }
}


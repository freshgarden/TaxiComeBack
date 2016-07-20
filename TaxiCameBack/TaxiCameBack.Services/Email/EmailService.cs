using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using TaxiCameBack.Core.Constants;

namespace TaxiCameBack.Services.Email
{
    public class EmailService : IEmailService
    {
        public List<Core.DomainModel.Email.Email> Emails { get; private set; }

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
                var sb = sr.ReadToEnd();
                sr.Close();
                sb = sb.Replace("#CONTENT#", content);
                sb = sb.Replace("#SITENAME#", AppConstants.SiteName);
                sb = sb.Replace("#SITEURL#", AppConstants.SiteUrl);
                if (!string.IsNullOrEmpty(to))
                {
                    to = $"<p>{to},</p>";
                    sb = sb.Replace("#TO#", to);
                }

                return sb;
            }
        }

        private void ProcessMail()
        {
            try
            {
                if (Emails != null && Emails.Any())
                {
                    var smtp = AppConstants.Smtp;
                    var smtpUsername = AppConstants.SmtpUsername;
                    var smtpPassword = AppConstants.SmtpPassword;
                    var smtpPort = AppConstants.SmtpPort;
                    var smtpEnableSsl = AppConstants.SmtpEnableSsl;
                    var fromEmail = AppConstants.SmtpFromEmail;

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

                    mySmtpClient.EnableSsl = smtpEnableSsl;
                    mySmtpClient.Port = smtpPort;

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
                        mySmtpClient.Send(msg);

                        emailsToDelete.Add(message);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new AccessViolationException();
            }
        }
    }
}


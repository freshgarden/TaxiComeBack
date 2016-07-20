using System.Collections.Generic;

namespace TaxiCameBack.Services.Email
{
    public interface IEmailService
    {
        void SendMail(Core.DomainModel.Email.Email email);
        void SendMail(List<Core.DomainModel.Email.Email> email);
        string EmailTemplate(string to, string content);
    }
}

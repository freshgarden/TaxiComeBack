using System;

namespace TaxiCameBack.Core.DomainModel.Email
{
    public class Email
    {
        public int Id { get; set; }
        public string EmailTo { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string NameTo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

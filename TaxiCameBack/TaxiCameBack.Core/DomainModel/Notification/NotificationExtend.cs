using System;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Notification
{
    public class NotificationExtend : BaseEntity
    {
        public NotificationExtend()
        {
            Id = GuidComb.GenerateComb();
        }

        [Key]
        public Guid Id { get; set; }
        public string BeginLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartDate { get; set; }
        public string Message { get; set; }

        public virtual Notification Notification { get; set; }
    }
}

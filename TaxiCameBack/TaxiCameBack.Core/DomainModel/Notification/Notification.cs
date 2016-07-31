using System;
using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Core.DomainModel.Notification
{
    public class Notification : BaseEntity
    {
        public Notification()
        {
            Id = GuidComb.GenerateComb();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Message { get; set; }
        public Guid UserId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public bool Viewed { get; set; }
    }
}

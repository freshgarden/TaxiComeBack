using System;
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.Notification
{
    public class Notification : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public int UserId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public bool Viewed { get; set; }
    }
}

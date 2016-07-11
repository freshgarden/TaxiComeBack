using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(11)]
        public int Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
    }
}
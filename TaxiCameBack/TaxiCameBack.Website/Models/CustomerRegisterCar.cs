using System.ComponentModel.DataAnnotations;
using TaxiCameBack.Website.App_LocalResources;
namespace TaxiCameBack.Website.Models
{
    public class CustomerRegisterCar
    {
        public int DriveId { get; set; }
        
        public string CustomerFullName { get; set; }
        
        public string CustomerPhoneNumber { get; set; }

        public string NearLocation { get; set; }    }
}
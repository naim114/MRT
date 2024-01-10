using System.ComponentModel.DataAnnotations;

namespace MRT.Models
{
    public class Booking
    {
        [Display(Name = "Booking Id")]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "From Station")]
        public string StationFrom { get; set; }

        [Required]
        [Display(Name = "To Station")]
        public string StationTo { get; set; }

        [Display(Name = "Ticket Type")]
        public bool IsOneWay { get; set; }

        [Required]
        [Display(Name = "Price Before Discount (RM)")]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Discount (%)")]
        public double DiscountPercentage { get; set; }

        [Required]
        [Display(Name = "Total Price (RM)")]
        public double TotalPrice { get; set; }
    }
}

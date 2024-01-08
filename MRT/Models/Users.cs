using System.ComponentModel.DataAnnotations;

namespace MRT.Models
{
    public class User
    {
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Identity Card")]
        public string IdentityCard { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; } = false;

        [Display(Name = "User Type")]
        public string? Type { get; set; } // Make Type nullable

        [Required]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one letter and one digit.")]
        public string Password { get; set; }
    }
}

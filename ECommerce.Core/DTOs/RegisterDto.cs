using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FullName name is required")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address name is required")]
        [MaxLength(100)]
        public string Address { get; set; }
    }
}

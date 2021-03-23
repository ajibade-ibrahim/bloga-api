using System.ComponentModel.DataAnnotations;

namespace Bloga.Models.Account
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(5, ErrorMessage = "Username should be at least 5 characters")]
        [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters")]
        public string Fullname { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Password should be at least 5 characters")]
        public string Password { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Username should be at least 5 characters")]
        [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters")]
        public string Username { get; set; }
    }
}
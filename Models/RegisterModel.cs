using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",ErrorMessage="Invalid Mail Address !")]
        [EmailUserUnique]

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string  ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}
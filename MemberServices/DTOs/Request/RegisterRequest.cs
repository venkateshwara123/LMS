using System.ComponentModel.DataAnnotations;

namespace MemberServices.DTOs.Request
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "Member"; // Default role is "Member"
    }
}

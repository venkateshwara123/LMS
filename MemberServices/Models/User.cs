using System.ComponentModel.DataAnnotations;

namespace MemberServices.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "Member";// e.g., "Admin", "Member"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

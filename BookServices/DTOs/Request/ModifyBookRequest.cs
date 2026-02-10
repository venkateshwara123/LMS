using System.ComponentModel.DataAnnotations;

namespace BookServices.DTOs.Request
{
    public class ModifyBookRequest
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Genre { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}

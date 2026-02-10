using System.ComponentModel.DataAnnotations;

namespace BookServices.DTOs.Request
{
    public class BookRequest
    {        
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Genre { get; set; }
    }
}

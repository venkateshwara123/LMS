using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookServices.Models
{
    public class Reservation
    {
        
        public int Id { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Username { get; set; }

        public Book Book { get; set; }
    }
}

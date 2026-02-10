using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookServices.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Username { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }=DateTime.UtcNow.AddDays(14);
    }
}

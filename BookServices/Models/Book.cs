using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookServices.Models
{
    public class Book
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage ="Please Enter the Title since its required.")]
        public string Title { get; set; }
        //[Required(ErrorMessage = "Please Enter the Author Name for this Book.")]
        public string Author { get; set; }
        //[Required(ErrorMessage = "Please Enter the Genre for this Book.")]
        public string Genre { get; set; }
        public bool IsAvailable { get; set; } = true;
        
    }
}

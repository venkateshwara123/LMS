namespace BookServices.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Username { get; set; }
    }
}

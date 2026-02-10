namespace BookServices.DTOs.Response
{
    public class ReservedResponse
    {
        public string Title { get; set; }        
        public string Author { get; set; }       
        public string Genre { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}

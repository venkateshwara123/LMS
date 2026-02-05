using BookServices.Models;
using Microsoft.EntityFrameworkCore;

namespace BookServices.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    
    }
}

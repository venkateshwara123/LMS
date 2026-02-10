using BookServices.Data;
using BookServices.DTOs.Request;
using BookServices.DTOs.Response;
using BookServices.Interfaces;
using BookServices.Models;
using Microsoft.EntityFrameworkCore;

namespace BookServices.Repository
{
    public class BookRepository: IBookRepository
    {
        private readonly BookDbContext _context;
        public BookRepository(BookDbContext context)
        {
            _context = context;
        }
        public async Task<Book> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }
        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id); ;
        }
        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.AsNoTracking().ToListAsync();
        }
        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
        

        public async Task DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Book>> GetReservedBooks(string username)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Username == username);
            if (reservation != null)
            {
                return await _context.Books.Where(b => b.Id == reservation.BookId).ToListAsync();
            }
            return new List<Book>();
        }
        public async Task ReserveAddBook(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }
        public async Task<Book> GetBookByAuthor(string author,string genre)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Author == author && b.Genre==genre);
        }
       public async Task<bool> IsBookReserved(int id,string Username)
        {
            bool alreadyReserved = await _context.Reservations
                 .AnyAsync(r => r.BookId == id && r.Username == Username);
            return alreadyReserved;
        }

        public async Task<PagedResponse<Book>> GetAllPageResponseAsync(int pageNumber, int pageSize)
        {
            var query = _context.Books.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(b => b.Id) // IMPORTANT: always order before Skip
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Book>
            {
                Items = items,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}

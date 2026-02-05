using BookServices.Data;
using BookServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookController(BookDbContext context)
        {
            _context = context;
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok(book);
        }

       // [Authorize(Roles = "Member")]
        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || !book.IsAvailable)
                return BadRequest("Book unavailable");

            var username = User.Identity!.Name;

            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r => r.BookId == id && r.Username == username);

            if (alreadyReserved)
                return BadRequest("Already reserved");

            book.IsAvailable = false;
            _context.Reservations.Add(new Reservation
            {
                BookId = id,
                Username = username
            });

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search(string author, string genre, int page = 1)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Contains(author));

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(b => b.Genre.Contains(genre));

            var result = await query
                .Skip((page - 1) * 5)
                .Take(5)
                .ToListAsync();

            return Ok(result);
        }

    }
}

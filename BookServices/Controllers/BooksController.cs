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
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BooksController(BookDbContext context)
        {
            _context = context;
        }

        // POST: api/Books To create new book Details
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Book>> AddBookDetails(Book book)
        {
            if (book == null)
                throw new ArgumentException("Invalid Book Details");

           await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooksById), new { id = book.Id }, book);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles ="Admin,Member")]
        public async Task<IActionResult> SearchBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Books.AsQueryable();
            var totalRecords = await query.CountAsync();
            if (totalRecords > 0)
            {
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var result = new PagedResult<Book>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                };

                return Ok(result);
            }
            else
                throw new KeyNotFoundException("Books UnAvailable");
        }

        // GET: api/Books
        [HttpGet]
        [Route("MyBooks",Name ="GetMyBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Book>>> GetMyBooks()
        {
            var username = User.Identity?.Name;

            var books = await _context.Reservations
                   .Where(r => r.Username == username)
                  .Include(r => r.Book)
                  .Select(r => r.Book)
                  .ToListAsync();
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id:int}",Name ="GetBooksById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Book>> GetBooksById(int id)
        {
            if (id <= 0)
                throw new ArgumentException($"No Book Found for the Id :{id}");            
            else
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    throw new KeyNotFoundException($"The Book with Id:{id} not Found. Please Try with valid Id.");
                }
                return book;
            }
        }

        // PUT: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}",Name ="UpdateBookDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBooks(int id, Book book)
        {
            if (id != book.Id)
            {
                throw new ArgumentException($"The  book Id {id} is unavailable to modified");
            }

            _context.Entry(book).State = EntityState.Modified;
             await _context.SaveChangesAsync();
             return Ok($"Successfully Modified the Book Details for Id: {id}");
        }

        // DELETE: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteBookById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Kindly check the Id");
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book Unavailable for this Id {id} for deletion");
            }
             _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok($"Successfully Deleted the Book Details for Id:{id}");
        }

        //Method to find the Book is Exists already or not
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Member")]
        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || !book.IsAvailable)
                throw new ArgumentException("Book unavailable");

            var username = User.Identity!.Name;

            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r => r.BookId == id && r.Username == username);

            if (alreadyReserved)
                throw new ArgumentException("Book already reserved by you");

            book.IsAvailable = false;
            _context.Reservations.Add(new Reservation
            {
                BookId = id,
                Username = username
            });

            await _context.SaveChangesAsync();
            return Ok($"Successfully Reserved by {username}");
        }

        [HttpGet("Easysearch",Name = "SearchBooksbyAuthor&Genre")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> SearchBooks([FromQuery] string author,[FromQuery] string genre)
        {
            if (!string.IsNullOrWhiteSpace(author) || !string.IsNullOrWhiteSpace(genre))
            {
                var query = _context.Books.AsQueryable();

                if (!string.IsNullOrWhiteSpace(author))
                    query = query.Where(b => b.Author.Contains(author));

                if (!string.IsNullOrWhiteSpace(genre))
                    query = query.Where(b => b.Genre.Contains(genre));

                var books = await query.ToListAsync();
                if(books.Any())
                return Ok(books);
                else 
                 throw new KeyNotFoundException($"The Book search with Author {author} and Genre {genre} was unavailable");
            }
            else
                throw new ArgumentException("Kindly Enter either Author or Genre to find the book");
        }

        

    }
}

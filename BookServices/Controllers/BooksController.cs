using BookServices.Data;
using BookServices.DTOs.Request;
using BookServices.DTOs.Response;
using BookServices.Interfaces;
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
        private readonly IBookService _context;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookService context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/Books To create new book Details
        [Authorize(Roles = "Admin")]
        [HttpPost("AddBookDetails")]  
        public async Task<ActionResult<Book>> AddBookDetails(Book book)
        {
            try
            {
                var response = await _context.AddBookDetails(book);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task <ActionResult<PagedResponse<Book>>> SearchBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = _context.GetAllBooks(new PagedRequest { PageNumber = pageNumber, PageSize = pageSize });
                return Ok(response.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Books
        [HttpGet]
        [Route("MyBooks",Name ="GetMyBooks")]
       public async Task<ActionResult<IEnumerable<Book>>> GetMyBooks()
        {
            try
            {
                var username = User.Identity?.Name;               
                _logger.LogInformation("Fetching reserved books for user: {Username}", username);
                var books = await _context.GetMyBooks(username);
                if (books == null || !books.Any())
                {
                    _logger.LogWarning("No reserved books found for user: {Username}", username);
                    throw new KeyNotFoundException($"No reserved books found for user: {username}");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reserved books for user: {Username}", User.Identity?.Name);
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Books/5
        [HttpGet("{id:int}",Name ="GetBooksById")]
        public async Task<ActionResult<Book>> GetBooksById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Book Id: {Id}", id);
                throw new ArgumentException($"No Book Found for the Id :{id}");
            }
            else
            {
                var book = await _context.GetBooksById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book Not Found for Id: {Id}", id);
                    throw new KeyNotFoundException($"The Book with Id:{id} not Found. Please Try with valid Id.");
                }
                _logger.LogInformation("Book Found: {Title} by {Author}", book.Title, book.Author);
                return book;
            }
        }

        // PUT: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}",Name ="UpdateBookDetails")]       
        public async Task<IActionResult> UpdateBooks(int id, Book book)
        {
            try
            {
                if (id != book.Id)
                {
                    _logger.LogWarning("Book Id Mismatch: {Id} does not match Book.Id: {BookId}", id, book.Id);
                    throw new ArgumentException($"The Book Id {id} does not match the Book Details provided");
                }
                bool result= await _context.UpdateBooks(book);
                if(!result)
                {
                    _logger.LogWarning("Failed to update book details for Id: {Id}", id);
                    throw new KeyNotFoundException($"Book Unavailable for this Id {id} for updation");
                }
                else
                {
                    _logger.LogInformation("Successfully Modified the Book Details for Id: {Id}", id);
                    return Ok($"Successfully Modified the Book Details for Id: {id}");                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book details for Id: {Id}", id);
                return BadRequest(ex.Message);
            }

        }

        // DELETE: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteBookById")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Book Id for Deletion: {Id}", id);
                throw new ArgumentException("Kindly check the Id");
            }
            bool result = await _context.DeleteBook(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete book details for Id: {Id}", id);
                throw new KeyNotFoundException($"Book Unavailable for this Id {id} for deletion");
            }
            else
            {
                _logger.LogInformation("Successfully Deleted the Book Details for Id: {Id}", id);
                return Ok($"Successfully Deleted the Book Details for Id:{id}");
            }
        }

        
        [Authorize(Roles = "Member")]
        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveBook(int id)
        {                       
            var username = User.Identity!.Name;
            var result = await _context.ReserveBook(id,username);
            if (!result)
            {
                _logger.LogWarning("User {Username} has already reserved book with Id: {Id}", username, id);
                throw new ArgumentException($"User {username} has already reserved this book.");
            }
            else
            {
                _logger.LogInformation("Book with Id: {Id} successfully reserved", id);
                return Ok($"Successfully Reserved by {username}");
            }
        }

        [HttpGet("Easysearch",Name = "SearchBooksbyAuthor&Genre")] 
        [Authorize]
        public async Task<IActionResult> SearchBooks([FromQuery] string author,[FromQuery] string genre)
        {          
            try
            {
                var response = await _context.GetBookByAuthor(author, genre);
                if (response == null)
                {
                    _logger.LogWarning("No books found for Author: {Author} and Genre: {Genre}", author, genre);
                    throw new KeyNotFoundException($"No books found for Author: {author} and Genre: {genre}");
                }
                _logger.LogInformation("Books found for Author: {Author} and Genre: {Genre}", author, genre);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books for Author: {Author} and Genre: {Genre}", author, genre);
                return BadRequest(ex.Message);
            }
        }        
    }
}

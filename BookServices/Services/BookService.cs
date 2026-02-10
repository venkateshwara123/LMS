using BookServices.DTOs.Request;
using BookServices.DTOs.Response;
using BookServices.Interfaces;
using BookServices.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace BookServices.Services
{
    public class BookService: IBookService
    {
        private readonly IBookRepository _context;
        private readonly ILogger<BookService> _logger;
        public BookService(IBookRepository context,ILogger<BookService> logger) 
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Book> AddBookDetails(Book book)
        {
            if (book == null)
                throw new ArgumentException("Invalid Book Details");

            _logger.LogInformation("Adding new book: {Title} by {Author}", book.Title, book.Author);
            return await _context.AddBookAsync(book);   
        }
        public async Task<PagedResponse<Book>> GetAllBooks(PagedRequest request)
        {
            _logger.LogInformation("Fetching all books with pagination. PageNumber: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);
            return await _context.GetAllPageResponseAsync(request.PageNumber,request.PageSize);                       
        }

        public async Task<IEnumerable<Book>> GetMyBooks(string username)
        {
            
            _logger.LogInformation("Fetching reserved books for user: {Username}", username);

            return await _context.GetReservedBooks(username); ;
        }

        public async Task<Book> GetBooksById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Book Id: {Id}", id);
                throw new ArgumentException($"No Book Found for the Id :{id}");
            }
            else
            {
                var book = await _context.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning("Book Not Found for Id: {Id}", id);
                    throw new KeyNotFoundException($"The Book with Id:{id} not Found. Please Try with valid Id.");
                }
                _logger.LogInformation("Book Found: {Title} by {Author}", book.Title, book.Author);
                return book;
            }
        }
        public async Task<bool> UpdateBooks(Book book)
        {                     
            await _context.UpdateBook(book);
            _logger.LogInformation("Successfully Modified the Book Details for Id: {Id}", book.Id);
            return true;
        }
        public async Task<bool> DeleteBook(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Book Id for Deletion: {Id}", id);
                return false;
                throw new ArgumentException("Kindly check the Id");
               
            }
            var book = await _context.GetBookByIdAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Book Not Found for Deletion with Id: {Id}", id);
                return false;
                throw new KeyNotFoundException($"Book Unavailable for this Id {id} for deletion");
            }
            await _context.DeleteBook(book);      
            _logger.LogInformation("Successfully Deleted the Book Details for Id: {Id}", id);
            return true;
        }

        public async Task<bool> ReserveBook(int id,string UserName)
        {
            var book = await _context.GetBookByIdAsync(id);
            if (book == null || !book.IsAvailable)
            {
                _logger.LogWarning("Attempt to reserve unavailable book with Id: {Id}", id);
                return false;
                throw new ArgumentException("Book unavailable");
            }
            bool alreadyReserved = await _context.IsBookReserved(id,UserName);
            if (alreadyReserved)
            {
                _logger.LogWarning("User {Username} has already reserved book with Id: {Id}", UserName, id);
                return false;
                throw new ArgumentException("Book already reserved by you");
            }
            book.IsAvailable = false;
            await _context.UpdateBook(book);
            var reservation = new Reservation
            {
                BookId = book.Id,
                Username = UserName,
                ReservationDate = DateTime.UtcNow
            };
            await _context.ReserveAddBook(reservation);
            _logger.LogInformation("Book with Id: {Id} successfully reserved", reservation.BookId);
            return true;

        }
        public async Task<Book> GetBookByAuthor(string author,string genre)
        {
            if (string.IsNullOrEmpty(author) && string.IsNullOrEmpty(genre))
            {
                _logger.LogWarning("Author or Genre is null or empty. Author: {Author}, Genre: {Genre}", author, genre);
                throw new ArgumentException("Author and Genre must be provided");
            }
            var book = await _context.GetBookByAuthor(author, genre);
            if (book == null)
            {
                _logger.LogInformation("No book found for Author: {Author} and Genre: {Genre}", author, genre);
                 throw new KeyNotFoundException($"No book found for Author: {author} and Genre: {genre}");
            }
            _logger.LogInformation("Book found for Author: {Author} and Genre: {Genre}. Title: {Title}", author, genre, book.Title);
            return book;
        }
    }
}

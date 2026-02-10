using BookServices.DTOs.Request;
using BookServices.DTOs.Response;
using BookServices.Models;

namespace BookServices.Interfaces
{
    public interface IBookService
    {
         Task<Book> GetBookByAuthor(string author, string genre);
        Task<Book> AddBookDetails(Book book);
        Task<IEnumerable<Book>> GetAllBooks(PagedRequest request);
        Task<IEnumerable<Book>> GetMyBooks(string username);
        Task<Book> GetBooksById(int id);
        Task<bool> DeleteBook(int id);
        Task<bool> ReserveBook(int id, string UserName);
        Task<bool> UpdateBooks(Book book);

    }
}

using BookServices.DTOs.Request;
using BookServices.Models;

namespace BookServices.Interfaces
{
    public interface IBookRepository
    {
         Task<Book> AddBookAsync(Book book);
         Task<Book> GetBookByIdAsync(int id);
         Task<List<Book>> GetAllBooksAsync();
         Task UpdateBook(Book book);
         Task<List<Book>> GetReservedBooks(string username);
         Task DeleteBook(Book book);
         Task ReserveAddBook(Reservation reservation);
        Task<bool> IsBookReserved(int id, string Username);
        Task<Book> GetBookByAuthor(string author, string genre);
    

    }
}

using BookServices.Models;
using BookServices.Repository;
using BookServicesTest.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class BookRepositoryTests
{
    [Fact]
    public async Task AddBookAsync_ShouldAddBook()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(AddBookAsync_ShouldAddBook));
        var repo = new BookRepository(context);

        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Genre = "Programming"
        };

        var result = await repo.AddBookAsync(book);

        result.Id.Should().BeGreaterThan(0);
        (await context.Books.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnBook()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(GetBookByIdAsync_ShouldReturnBook));
        var book = new Book { Title = "DDD", Author = "Evans", Genre = "Tech" };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var repo = new BookRepository(context);

        var result = await repo.GetBookByIdAsync(book.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(book.Id);
    }
    
    [Fact]
    public async Task GetReservedBooks_ShouldReturnEmpty_WhenNoReservation()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(GetReservedBooks_ShouldReturnEmpty_WhenNoReservation));
        var repo = new BookRepository(context);

        var result = await repo.GetReservedBooks("unknown");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReserveAddBook_ShouldCreateReservation()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(ReserveAddBook_ShouldCreateReservation));
        var repo = new BookRepository(context);

        var reservation = new Reservation
        {
            BookId = 1,
            Username = "user1"
        };

        await repo.ReserveAddBook(reservation);

        (await context.Reservations.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetBookByAuthor_ShouldReturnMatchingBook()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(GetBookByAuthor_ShouldReturnMatchingBook));
        context.Books.Add(new Book
        {
            Author = "Author1",
            Genre = "SciFi",
            Title = "Book"
        });
        await context.SaveChangesAsync();

        var repo = new BookRepository(context);

        var result = await repo.GetBookByAuthor("Author1", "SciFi");

        result.Should().NotBeNull();
        result!.Author.Should().Be("Author1");
    }

    [Fact]
    public async Task IsBookReserved_ShouldReturnTrue_WhenReserved()
    {
        using var context = BookDBContextHelper.CreateContext(nameof(IsBookReserved_ShouldReturnTrue_WhenReserved));
        context.Reservations.Add(new Reservation
        {
            BookId = 10,
            Username = "user1"
        });
        await context.SaveChangesAsync();

        var repo = new BookRepository(context);

        var result = await repo.IsBookReserved(10, "user1");

        result.Should().BeTrue();
    }
}

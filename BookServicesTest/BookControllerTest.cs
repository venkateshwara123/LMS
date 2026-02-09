using BookServices.Controllers;
using BookServices.Data;
using BookServices.Models;
using BookServicesTest.Tests.Helpers;

//using BookServices.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;


namespace BookServicesTest
{
    public class BookControllerTest
    {
        private readonly BookDbContext _context;
        private readonly BooksController _controller;
        private readonly Mock<ILogger<BooksController>> _loggerMock;

        public BookControllerTest()
        {
            _context = DbContextHelper.CreateInMemoryContext();
            _loggerMock = new Mock<ILogger<BooksController>>();
            _controller = new BooksController(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task GetBooksById_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var book = new Book
            {
                Title = "Domain Driven Design",
                Author = "Eric Evans",
                Genre = "Architecture",
                IsAvailable = true
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBooksById(book.Id);

            // Assert
            var okResult = Assert.IsType<ActionResult<Book>>(result);
            Assert.Equal("Domain Driven Design", result.Value.Title);
        }

        [Fact]
        public async Task GetBooksById_ShouldThrow_WhenBookNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _controller.GetBooksById(999));
        }
        [Fact]
        public async Task GetAllBooks_ShouldReturnPagedResult()
        {
            // Arrange
            _context.Books.AddRange(
                new Book { Title = "Book1", Author = "A", Genre = "G1", IsAvailable = true },
                new Book { Title = "Book2", Author = "B", Genre = "G2", IsAvailable = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.SearchBooks(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

         }
}
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using BookServices.Controllers;
using BookServices.Data;
using BookServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookServicesTest
{
    public class BookControllerTest
    {
        private static BookDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BookDbContext(options);
        }

        // now accepts an optional role; include role claim when needed by tested endpoints
        private static BooksController CreateControllerWithUser(BookDbContext context, string username, string? role = null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            ClaimsIdentity identity;
            if (!string.IsNullOrWhiteSpace(role))
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }, "mock");
            }
            else
            {
                identity = new ClaimsIdentity(claims, "mock");
            }

            var user = new ClaimsPrincipal(identity);
            var controller = new BooksController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            return controller;
        }

        [Fact]
        public async Task AddBookDetails_ReturnsCreatedAndPersists()
        {
            await using var context = CreateInMemoryContext();
            var controller = CreateControllerWithUser(context, "admin", "Admin");

            var book = new Book
            {
                Title = "Unit Testing in .NET",
                Author = "Tester",
                Genre = "Tech",
                IsAvailable = true
            };

            var result = await controller.AddBookDetails(book);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(BooksController.GetBooksById), created.ActionName);

            var persisted = await context.Books.FirstOrDefaultAsync(b => b.Title == "Unit Testing in .NET");
            Assert.NotNull(persisted);
            Assert.Equal("Tester", persisted.Author);
        }

        [Fact]
        public async Task SearchBooks_ReturnsPagedResult_WhenBooksExist()
        {
            await using var context = CreateInMemoryContext();
            for (int i = 1; i <= 5; i++)
            {
                context.Books.Add(new Book { Title = $"Book{i}", Author = "A", Genre = "G" });
            }
            await context.SaveChangesAsync();

            var controller = CreateControllerWithUser(context, "member", "Member");
            var result = await controller.SearchBooks(pageNumber: 1, pageSize: 2);

            var ok = Assert.IsType<OkObjectResult>(result);
            var paged = Assert.IsType<PagedResult<Book>>(ok.Value);

            Assert.Equal(2, paged.Items.Count());
            Assert.Equal(1, paged.PageNumber);
            Assert.Equal(2, paged.PageSize);
            Assert.Equal(5, paged.TotalRecords);
        }

        [Fact]
        public async Task GetBooksById_ReturnsBook_WhenExists_And_BadRequest_ForInvalidId()
        {
            await using var context = CreateInMemoryContext();
            var book = new Book { Title = "FindMe", Author = "A" };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var controller = CreateControllerWithUser(context, "member");

            var goodResult = await controller.GetBooksById(book.Id);
            Assert.NotNull(goodResult.Value);
            Assert.Equal("FindMe", goodResult.Value.Title);

            var badResult = await controller.GetBooksById(0);
            Assert.IsType<BadRequestObjectResult>(badResult.Result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenDeleted()
        {
            await using var context = CreateInMemoryContext();
            var book = new Book { Title = "ToDelete", Author = "A" };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var controller = CreateControllerWithUser(context, "admin", "Admin");
            var result = await controller.DeleteBook(book.Id);

            Assert.IsType<NoContentResult>(result);
            var exists = await context.Books.FindAsync(book.Id);
            Assert.Null(exists);
        }

        [Fact]
        public async Task ReserveBook_Succeeds_WhenAvailable_And_Fails_WhenAlreadyReserved()
        {
            await using var context = CreateInMemoryContext();
            var book = new Book { Title = "Reservable", Author = "A", IsAvailable = true };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // success case
            var controller = CreateControllerWithUser(context, "user1", "Member");
            var okResult = await controller.ReserveBook(book.Id);
            Assert.IsType<OkResult>(okResult);

            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.False(updatedBook.IsAvailable);
            var reservation = await context.Reservations.FirstOrDefaultAsync(r => r.Username == "user1" && r.BookId == book.Id);
            Assert.NotNull(reservation);

            // attempt to reserve again by same user -> BadRequest (already reserved)
            var controllerSameUser = CreateControllerWithUser(context, "user1", "Member");
            var alreadyReservedResult = await controllerSameUser.ReserveBook(book.Id);
            Assert.IsType<BadRequestObjectResult>(alreadyReservedResult);
        }
    }
}
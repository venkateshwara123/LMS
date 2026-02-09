using BookServices.Data;
using Microsoft.EntityFrameworkCore;

namespace BookServicesTest.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static BookDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new BookDbContext(options);
        }
    }
}

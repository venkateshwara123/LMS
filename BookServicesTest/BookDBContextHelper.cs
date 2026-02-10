using BookServices.Data;
using Microsoft.EntityFrameworkCore;

namespace BookServicesTest.Tests.Helpers
{
    public static class BookDBContextHelper
    {
        public static BookDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new BookDbContext(options);
        }
    }
}

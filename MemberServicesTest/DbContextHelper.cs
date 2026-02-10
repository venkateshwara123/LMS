
using MemberServices.Data;
using Microsoft.EntityFrameworkCore;

namespace MemberServicesTest
{
   public static class DbContextHelper
    {
        public static AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }
    }

}

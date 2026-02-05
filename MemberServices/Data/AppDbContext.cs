using MemberServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberServices.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> User { get; set; }
    }
}

using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Data // ✅ Đúng theo tên project
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
    }
}
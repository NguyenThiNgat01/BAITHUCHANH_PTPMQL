using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DaiLy> DaiLy { get; set; }    // đã có
        public DbSet<Person> People { get; set; }  // 👈 thêm dòng này để sửa lỗi PersonController
    }
}
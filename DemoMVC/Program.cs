using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using MvcMovie.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));


// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// Cấu hình DbContext với SQLite
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext")));

var app = builder.Build();

// Cấu hình pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HSTS = bảo mật HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Cấu hình định tuyến
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
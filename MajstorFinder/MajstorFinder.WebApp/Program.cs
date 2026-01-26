using MajstorFinder.DAL.DBC;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DB (RWA context)
builder.Services.AddDbContext<MajstoriDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session (za spremiti userId/role)
builder.Services.AddSession();

// Cookie auth (opcionalno, ali preporuka)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
    });

builder.Services.AddAuthorization();

// BLL servisi (za poèetak Auth)
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
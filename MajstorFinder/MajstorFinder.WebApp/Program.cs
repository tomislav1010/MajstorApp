using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// DbContext (RWA baza)
builder.Services.AddDbContext<MajstoriDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// BLL servisi
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITvrtkaService, TvrtkaService>();
builder.Services.AddScoped<ILokacijaService, LokacijaService>();
builder.Services.AddScoped<IVrstaRadaService, VrstaRadaService>();
builder.Services.AddScoped<IZahtjevService, ZahtjevService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
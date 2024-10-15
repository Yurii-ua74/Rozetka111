using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Rozetka.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rozetka.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Rozetka.Servises;


var builder = WebApplication.CreateBuilder(args);
int TimeBeforeLogout = 60;
// Додавання  MVC
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataService, DataService>();

////////// Налаштування сессій ////////////
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(TimeBeforeLogout); // час очикування
    options.Cookie.HttpOnly = true;  // Захист від JS-атак
    options.Cookie.IsEssential = true;
});

////////// налаштування для Identity ///////////
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "@.abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяІіЇїЄєҐґҐ";
    // політика безпеки
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

////////// Налаштування аутентифікації ////////////
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromSeconds(TimeBeforeLogout); // час до виходу
        options.SlidingExpiration = false; // Оновлення часу при активності
    });
//builder.Services.AddAutoMapper(typeof(ProductProfile)); // Налаштовує AutoMapper, щоб використовувати профіль конфігурації, визначений в ProductProfile



////////// Register DbContext and services ////////////
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////////// Add ASP.NET Identity services //////////////
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Ваші опції для користувачів і паролів
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor(); // для получения данных о пользователе в FavoritesCountViewComponent

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    })

     .AddFacebook(facebookOptions =>
     {
         facebookOptions.AppId = builder.Configuration["Facebook:AppId"];
         facebookOptions.AppSecret = builder.Configuration["Facebook:AppSecret"];
     });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // мідлвер для роботи з сесіями

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseMiddleware<AuthSessionMiddleware>();   // строка видає помилку

// маршрут за дефолтом
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// додано маршрут для пошуку сторінок по категоріям
//app.MapControllerRoute(
//    name: "category",
//    pattern: "{controller=Home}/{action=Category}/{category}",
//    defaults: new { controller = "Home", action = "Category" },
//    constraints: new { category = @"^[^/]+$" }); // якщо параметр category не є порожній




app.Run();



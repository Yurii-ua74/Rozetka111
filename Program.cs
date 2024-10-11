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


var builder = WebApplication.CreateBuilder(args);
int TimeBeforeLogout = 60;
// ���������  MVC
builder.Services.AddControllersWithViews();


////////// ������������ ����� ////////////
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(TimeBeforeLogout); // ��� ����������
    options.Cookie.HttpOnly = true;  // ������ �� JS-����
    options.Cookie.IsEssential = true;
});

////////// ������������ ��� Identity ///////////
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "@.abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789�����Ũ�������������������������������������������������������������������";
    // ������� �������
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

////////// ������������ �������������� ////////////
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromSeconds(TimeBeforeLogout); // ��� �� ������
        options.SlidingExpiration = false; // ��������� ���� ��� ���������
    });
//builder.Services.AddAutoMapper(typeof(ProductProfile)); // ��������� AutoMapper, ��� ��������������� ������� ������������, ���������� � ProductProfile



////////// Register DbContext and services ////////////
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////////// Add ASP.NET Identity services //////////////
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor(); // ��� ��������� ������ � ������������ � FavoritesCountViewComponent

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

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // ������ ��� ������ � ������

//app.UseMiddleware<AuthSessionMiddleware>();   // ������ ���� �������

// ������� �� ��������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ������ ������� ��� ������ ������� �� ���������
//app.MapControllerRoute(
//    name: "category",
//    pattern: "{controller=Home}/{action=Category}/{category}",
//    defaults: new { controller = "Home", action = "Category" },
//    constraints: new { category = @"^[^/]+$" }); // ���� �������� category �� � �������




app.Run();



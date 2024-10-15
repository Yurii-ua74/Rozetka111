using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data.Entity;
using System.Reflection.Emit;


//using RozetkaDatabase.Models;

namespace Rozetka.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Настройка для таблицы ProductImages
            builder.Entity<ProductImage>()
                .ToTable("ProductImages");

            // Настройка типа данных для поля Price в таблице Product
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 0)"); // Устанавливаем тип данных decimal(18, 0)

            builder.Entity<Product>()
                .Property(p => p.Rating)
                .HasColumnType("decimal(2, 1)"); // 1 целое и 1 дробное

            // Явно указываем связь между Cart и CartItem
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);  // Если корзина удаляется, удаляем связанные элементы

            //////////////////////////////////////

            builder.Entity<SubChildCategory>()
            .HasOne(s => s.Childcategory)
            .WithMany(c => c.SubChildCategories)  // Убедитесь, что это соответствует
            .HasForeignKey(s => s.ChildCategoryId);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Childcategory> Childcategories{ get; set; }
        public DbSet<SubChildCategory> SubChildCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ShoppingList> ShoppingList { get; set; }
        //public DbSet<LoginJournalItem> LoginJournal { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasDefaultSchema("rozetka_db");
        //}
    }
}

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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishList { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ShoppingList> ShoppingList { get; set; }
        //public DbSet<LoginJournalItem> LoginJournal { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasDefaultSchema("rozetka_db");
        //}
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data.Entity;


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

            builder.Entity<ProductImage>()
        .ToTable("ProductImages");

            //////////////////////////////////////
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Childcategory> Childcategories{ get; set; }
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

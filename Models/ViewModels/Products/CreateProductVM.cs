using Microsoft.AspNetCore.Mvc.Rendering;
using Rozetka.Data.Entity;


namespace Rozetka.Models.ViewModels.Products
{
    public class CreateProductVM
    {
        public SelectList? Brands { get; set; }

        public SelectList? Childcategories { get; set; }

        public Product Product { get; set; } = new Product();
    }
}

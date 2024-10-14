using Microsoft.AspNetCore.Mvc.Rendering;
using Rozetka.Data.Entity;


namespace Rozetka.Models.ViewModels.Products
{
    public class CreateProductVM
    {
        public SelectList? ProductTypes { get; set; }

        public SelectList? Brands { get; set; }

        public SelectList? ProductColors { get; set; }

        public SelectList? Categories { get; set; }

        public SelectList? Childcategories { get; set; }

        public SelectList? SubChildCategories { get; set; }

        public Product Product { get; set; } = new Product();


        public int CategoryId { get; set; }
        public int ChildcategoryId { get; set; }
        public int SubChildCategoryId { get; set; }
    }
}

using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Products
{
    public class ProductsViewModel
    {
        public IEnumerable<Product>? Products { get; set; }
        public string? Category { get; set; }
        public string? ChildCategory { get; set; }
        public string? SubChildCategory { get; set; }
    }
}

using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Products
{
    public class GetProductViewModel
    {
        public Product? Product { get; set; }
        public string? SellerName { get; set; }
        public IEnumerable<Product>? AdvertisingProducts { get; set; } // дополнительные товары на странице

    }
}

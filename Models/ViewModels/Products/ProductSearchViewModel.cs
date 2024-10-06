using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Products
{
    public class ProductSearchViewModel
    {
        public IEnumerable<Product>? SearchingResults { get; set; }       
    }
}

using Rozetka.Data.Entity;

namespace Rozetka.Servises
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(
            string title,
            string description,
            string characteristic,
            decimal price,
            Category category,
            Childcategory childcategory,
            SubChildCategory subchildcategory
            );
    }
}

using Rozetka.Data.Entity;

namespace Rozetka.Servises
{
    public interface IProductColorService
    {
        Task AddImagesAsync(int productId, ProductColor productColor);
    }
}

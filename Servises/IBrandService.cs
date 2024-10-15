using Rozetka.Data.Entity;

namespace Rozetka.Servises
{
    public interface IBrandService
    {
        Task AddImagesAsync(int productId, Brand productBrand);
    }
}

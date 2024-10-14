using Microsoft.AspNetCore.Mvc;
using Rozetka.Data.Entity;

namespace Rozetka.Servises
{
    public interface IProductTypeService
    {       
        Task AddImagesAsync(int productId, ProductType productType);
    }
}

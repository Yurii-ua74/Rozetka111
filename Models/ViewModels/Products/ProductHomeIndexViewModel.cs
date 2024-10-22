using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Products
{
    public class ProductHomeIndexViewModel
    {
        public IEnumerable<Product>? GetProductsRating { get; set; } //Рекомендуємо для вас
        public IEnumerable<Product>? GetProductsRandom { get; set; } //Більше товарів для вибору
        public IEnumerable<Product>? GetProductsActions { get; set; } //Акційні пропозиції
        public IEnumerable<Product>? GetProductsFavorites { get; set; } //Найчастіше додають до списку бажань
        public IEnumerable<Product>? GetProductsGarments { get; set; } //Актуальне в категорії одяг
        public IEnumerable<Product>? GetProductsShopping { get; set; } //Користуються попитом 

    }

    //public class ProductHomeIndexViewModel
    //{
    //    public ICollection<Product>? GetProductsRating { get; set; } //Рекомендуємо для вас
    //    public ICollection<Product>? GetProductsRandom { get; set; } //Більше товарів для вибору
    //    public ICollection<Product>? GetProductsActions { get; set; } //Акційні пропозиції
    //    public ICollection<Product>? GetProductsFavorites { get; set; } //Найчастіше додають до списку бажань
    //    public ICollection<Product>? GetProductsShopping { get; set; } //Користуються попитом 

    //}
}

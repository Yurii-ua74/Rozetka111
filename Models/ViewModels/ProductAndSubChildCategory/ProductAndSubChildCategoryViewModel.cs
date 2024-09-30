using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.ProductAndSubChildCategory
{
    public class ProductAndSubChildCategoryViewModel
    {
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<SubChildCategory>? SubChildCategories { get; set; }
    }
}

namespace Rozetka.Data.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? Сharacteristic { get; set; }
        public decimal Price { get; set; }
        public decimal? Rating { get; set; } //середній рейтинг
        public int Amount { get; set; } = 0; //кількість голосів

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int? ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }
        public int? ChildcategoryId { get; set; }
        public Childcategory? Childcategory { get; set; }
        public int? ProductColorId { get; set; }
        public ProductColor? ProductColors { get; set; }
        public int? QuantityInStock { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public int SubChildCategoryId { get; set; }
        public SubChildCategory? SubChildCategory { get; set; }
    }

}

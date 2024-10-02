namespace Rozetka.Data.Entity
{
    public class ProductColor
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string HexCode { get; set; } = default!;
        public ICollection<Product>? Products { get; set; }
    }
}

namespace Rozetka.Data.Entity
{
    public class Childcategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}

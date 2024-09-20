namespace Rozetka.Data.Entity
{
    public class SubChildCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ChildCategoryId { get; set; }

        // Навігаційні властивості
        public Childcategory? Childcategory { get; set; }

        // Колекція продуктів, які належать до цієї підкатегорії
        public ICollection<Product>? Products { get; set; }
    }
}

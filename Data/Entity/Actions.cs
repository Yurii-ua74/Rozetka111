namespace Rozetka.Data.Entity
{
    public class Actions
    {
        public int Id { get; set; }
        public int ProductId { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? NewPrice { get; set; }
        public int? Discount { get; set; }
        // Связь с продуктом
        public Product? Product { get; set; }
    }
}

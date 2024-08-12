namespace Rozetka.Data.Entity
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; } 
        public Product Product { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;
        public int Rating { get; set; } = 5;
        public string? Comment { get; set; }
    }
}

namespace Rozetka.Data.Entity
{
    public class ShoppingList //список покупок
    {
        public int Id { get; set; }        
        public DateTime DatePurchase { get; set; }
        public decimal? TotalPrice { get; set; }

        public int CartId { get; set; } = default!;
        public Cart? Cart { get; set; } //public ICollection<Cart>? Cart { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }        
    }
}

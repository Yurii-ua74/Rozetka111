namespace Rozetka.Data.Entity
{
    public class ShoppingList //список покупок
    {
        public int Id { get; set; }
        public ICollection<Cart>? Cart { get; set; }
        public DateTime DatePurchase { get; set; }
        public decimal? TotalPrice { get; set; }
        public string UserId { get; set; } = default!;
        public User? User { get; set; }
    }
}

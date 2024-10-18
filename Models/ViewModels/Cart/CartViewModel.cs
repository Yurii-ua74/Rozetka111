using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public ICollection<CartItem> CartItems { get; set; } = default!;
        public double TotalPrice { get; set; }
        // Данные пользователя
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public ICollection<CartItem> CartItems { get; set; }
        public double TotalPrice { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Rozetka.Data.Entity
{  
    [Keyless]
    public class CartItem
    {
        public Product Product { get; set; } = default!;

        public int Count { get; set; }

        public double TotalPrice => (double)(Product.Price * Count);
    }
}

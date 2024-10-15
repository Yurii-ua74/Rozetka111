using Microsoft.EntityFrameworkCore;

namespace Rozetka.Data.Entity
{  
    //[Keyless]
    //public class CartItem
    //{
    //    public Product Product { get; set; } = default!;

    //    public int Count { get; set; }

    //    public double TotalPrice => (double)(Product.Price * Count);
    //}



    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; } = default!;
        public int Count { get; set; } = 1;
        public int CartId { get; set; }  // Связь с корзиной       

        // Связь с продуктом
        public Product? Product { get; set; }
        // Связь с корзиной       
        public Cart Cart { get; set; } = default!;

        public double? TotalPrice => Product != null ? (double)(Product.Price * Count) : null;
    }
   

}

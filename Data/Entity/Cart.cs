using System.ComponentModel.DataAnnotations.Schema;

namespace Rozetka.Data.Entity
{
     public class Cart
     {
        public int Id { get; set; }    
        // Связь с пользователем
        public string UserId { get; set; } = default!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; } = 0;
        public bool IsActive { get; set; } = false;
        public User? User { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        // Расчет общей стоимости корзины
        public double TotalCartPrice => Items.Sum(item => item.TotalPrice ?? 0);        
     }




    //public class Cart
    //{
    //    public int Id { get; set; }
    //    public int ProductId { get; set; }
    //    public int Quantity { get; set; } = 1;
    //    public string UserId { get; set; } = default!;
    //    public User? User { get; set; }
    //    public Product? Product { get; set; }

    //    [NotMapped]
    //    // властивість для прив'язки продуктів у кошику
    //    protected ICollection<CartItem> items { get; set; } = new List<CartItem>();
    //    [NotMapped]
    //    public IEnumerable<CartItem> CartItems => items;




    //    // Конструктор для Entity Framework
    //    public Cart()
    //    {
    //        this.items = new List<CartItem>();
    //    }

    //    // Конструктор для ініціалізації кошика з існуючими елементами
    //    public Cart(IEnumerable<CartItem> items)
    //    {
    //        this.items = new List<CartItem>(items);
    //    }




    //    // Метод для отримання загальної вартості
    //    public double GetTotalPrice()
    //    {
    //        //return items.Sum(t => t.TotalPrice);
    //        return (double)CartItems.Sum(item => item.Count * item.Product.Price);
    //    }

    //    // Метод для видалення продукту з кошика
    //    public bool RemoveFromCart(CartItem item)
    //    {
    //        return RemoveFromCart(item.Product.Id);
    //    }
    //    public bool RemoveFromCart(int id)
    //    {
    //        CartItem? cartItem = items.FirstOrDefault(t => t.Product.Id == id);
    //        if (cartItem != null)
    //            return items.Remove(cartItem);
    //        return false;
    //    }


    //    // Збільшення кількості продукту  // станом на 12,08,2024 не використовується
    //    public void IncCount(int id)
    //    {
    //        CartItem? item = items.FirstOrDefault(t => t.Product.Id == id);
    //        if (item != null)
    //            AddToCart(item);
    //    }
    //    public void AddToCart(CartItem item)
    //    {
    //        CartItem? cartItem = items.FirstOrDefault(t => t.Product.Id == item.Product.Id);
    //        if (cartItem != null)
    //        {
    //            cartItem.Count += 1;
    //        }
    //        else
    //            items.Add(item);
    //    }

    //    // Метод для підрахунку загальної кількості товарів
    //    public int GetTotalCount()
    //    {
    //        return items.Sum(i => i.Count);
    //    }

    //    // Зменшення кількості продукту   // станом на 12,08,2024 не використовується
    //    public void DecCount(int id)
    //    {
    //        CartItem? item = items.FirstOrDefault(t => t.Product.Id == id);
    //        if (item != null)
    //        {
    //            item.Count--;
    //            if (item.Count == 0)
    //            {
    //                RemoveFromCart(item);
    //            }
    //        }
    //    }
    //}
}

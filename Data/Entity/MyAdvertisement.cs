namespace Rozetka.Data.Entity
{
    public class MyAdvertisement
    {
            public int Id { get; set; }          // Первинний ключ            
            public DateTime DatePosted { get; set; } = DateTime.Now;  // Дата публікації           

            // Foreign Key для користувача
            public string UserId { get; set; }   // ID користувача (повинно бути string для зв'язку з Identity)
            public virtual User User { get; set; }  // Зв'язок з користувачем

            // Foreign Key для товару
            public int ProductId { get; set; }   // ID товару
            public virtual Product Product { get; set; }  // Зв'язок з таблицею товарів        
    } 
}


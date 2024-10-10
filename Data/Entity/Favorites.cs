using System.ComponentModel.DataAnnotations.Schema;

namespace Rozetka.Data.Entity
{
    [Table("Favorites")]  // Указываем новое имя таблицы
    public class Favorites
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = default!;

        public User? User { get; set; }
        public Product? Product { get; set; }  // Связь с продуктом
    }
}

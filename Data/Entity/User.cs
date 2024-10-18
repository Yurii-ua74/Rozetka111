using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozetka.Data.Entity
{
    public class User : IdentityUser
    {       
        public string? FirstName { get; set; }
        public string? LastName { get; set; }



        [NotMapped] // Це поле не буде зберігатися в базі даних
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Новий пароль є обов'язковим.")]
        [StringLength(100, ErrorMessage = "Пароль має містити принаймні {2} символів.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Будь ласка, підтвердіть новий пароль.")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; }



        public DateTime RegisterDt { get; set; }
        
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Favorites>? Favorites { get; set; }
        public ICollection<Cart>? Cart { get; set; } // нужно public Cart? Cart { get; set; } 
        public ICollection<ShoppingList>? ShoppingList { get; set; }
        public virtual ICollection<MyAdvertisement> MyAdvertisements { get; set; }  // Зв'язок з оголошеннями
    }
}

using Microsoft.AspNetCore.Identity;

namespace Rozetka.Data.Entity
{
    public class User : IdentityUser
    {       
        public string? FirstName { get; set; }
        public string? LastName { get; set; }    
       
        public DateTime RegisterDt { get; set; }
        
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<WishList>? WishList { get; set; }
        public ICollection<Cart>? Cart { get; set; }
        public ICollection<ShoppingList>? ShoppingList { get; set; }
    }
}

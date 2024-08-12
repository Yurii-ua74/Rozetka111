namespace Rozetka.Data.Entity
{
    public class WishList
    {
        public Guid Id { get; set; }
        public Guid? IdProduct { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}

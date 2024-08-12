namespace Rozetka.Data.Entity
{
    public class LoginJournalItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime Moment { get; set; }
    }
}

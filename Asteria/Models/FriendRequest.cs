namespace Asteria.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
        public string? ReceiverId { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }
        public DateTime DateSent { get; set; }
    }
}

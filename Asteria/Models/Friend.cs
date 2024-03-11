namespace Asteria.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string? FriendshipId { get; set; }
        public virtual ApplicationUser? Friendship { get; set; }
        public DateTime DateAccepted { get; set; }
    }
}

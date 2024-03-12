using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Asteria.Models
{
    //no-research idea: when sending message, add to each user's contact list the message and which user sent it
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Message must not be empty")]
        public string Text { get; set; }
        public DateTime Received { get; set; }
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
        public string? ReceiverId { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }
    }
}

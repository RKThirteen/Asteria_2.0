using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asteria.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Collection>? Collections { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        //friends
        public virtual ICollection<Friend> Friends { get; set; }

        //contacts-->anyone who sent messages to our user/anyone to who the user sent a message to
        public virtual List<ApplicationUser>? ContactList { get; set; } = new List<ApplicationUser>();

        //friend requests
        public virtual ICollection<FriendRequest> FriendRequestsSent { get; set; }
        public virtual ICollection<FriendRequest> FriendRequestsReceived { get; set; }

        //temp

        public virtual ICollection<Friend> Friend1 { get; set; }
        public virtual ICollection<Friend> Friend2 { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [StringLength(20, ErrorMessage = "Username-ul trebuie sa fie intre 3 si 20 de caractere!")]
        [MinLength(3, ErrorMessage = "Username-ul trebuie sa fie intre 3 si 20 de caractere!")]
        public string? Nickname { get; set; }
        public byte[]? ProfilePic { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}

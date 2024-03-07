﻿using Microsoft.AspNetCore.Identity;
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
        public virtual List<ApplicationUser>? Friends { get; set; } = new List<ApplicationUser>();

        //contacts-->anyone who sent messages to our user/anyone to who the user sent a message to
        public virtual List<ApplicationUser>? ContactList { get; set; } = new List<ApplicationUser>();

        //friend requests
        public virtual List<ApplicationUser>? FriendRequests { get; set; } = new List<ApplicationUser>();

        public virtual List<ApplicationUser>? Pending { get; set; } = new List<ApplicationUser>();

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

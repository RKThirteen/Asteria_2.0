using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asteria.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title must exist")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description must exist")]
        public string Description { get; set; }

        [Required(ErrorMessage = "You can't post something without content!")]
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public int? CollectionId { get; set; }

        public int Likes { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<PostCollection>? PostCollections { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

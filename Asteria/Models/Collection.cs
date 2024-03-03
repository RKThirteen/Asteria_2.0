using System.ComponentModel.DataAnnotations;

namespace Asteria.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CollectionName { get; set; }

        [MaxLength(20, ErrorMessage = "Descrierea nu trebuie sa depaseasca 20 de caractere")]
        public string? Description { get; set; }

        public virtual ICollection<PostCollection>? PostCollections { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

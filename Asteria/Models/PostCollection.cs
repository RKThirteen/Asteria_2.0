namespace Asteria.Models
{
	public class PostCollection
	{
        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? CollectionId { get; set; }
        public virtual Post? Post { get; set; }
        public virtual Collection? Collection { get; set; }
        public DateTime PostDate { get; set; }
    }
}

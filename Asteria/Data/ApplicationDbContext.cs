using Asteria.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asteria.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<PostCollection> PostCollections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostCollection>()
                .HasKey(ab => new { ab.Id, ab.PostId, ab.CollectionId });


            modelBuilder.Entity<PostCollection>()
                .HasOne(ab => ab.Post)
                .WithMany(ab => ab.PostCollections)
                .HasForeignKey(ab => ab.PostId);

            modelBuilder.Entity<PostCollection>()
                .HasOne(ab => ab.Collection)
                .WithMany(ab => ab.PostCollections)
                .HasForeignKey(ab => ab.CollectionId);


        }
    }
}
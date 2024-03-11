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
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friend> Friends { get; set; }

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

            modelBuilder.Entity<FriendRequest>()
                .HasKey(f => new { f.Id, f.SenderId, f.ReceiverId });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(f => f.Sender)
                .WithMany(u => u.FriendRequestsSent)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(f => f.Receiver)
                .WithMany(u => u.FriendRequestsReceived)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasKey(f => new { f.Id, f.UserId, f.FriendshipId });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friend1)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Friendship)
                .WithMany(u => u.Friend2)
                .HasForeignKey(f => f.FriendshipId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
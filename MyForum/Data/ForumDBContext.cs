using Data.Models;
using Microsoft.EntityFrameworkCore;
using MyForum.Data.Models;

namespace Data
{
    public class ForumDBContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Threads> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ForumDBContext(DbContextOptions<ForumDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = 1, Name = "admin" };
            Role moderRole = new Role { Id = 2, Name = "moderator" };
            Role userRole = new Role { Id = 3, Name = "user" };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole,moderRole, userRole });
        }
    }
}

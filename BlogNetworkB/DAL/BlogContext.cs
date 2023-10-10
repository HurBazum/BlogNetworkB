using Microsoft.EntityFrameworkCore;
using BlogNetworkB.DAL.Enteties;

namespace BlogNetworkB.DAL
{
    public class BlogContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }
    }
}
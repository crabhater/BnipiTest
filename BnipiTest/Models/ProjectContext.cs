using Microsoft.EntityFrameworkCore;

namespace BnipiTest.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base (options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<DesignObject> DesignObjects { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Mark> Marks { get; set; }

        public ProjectContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

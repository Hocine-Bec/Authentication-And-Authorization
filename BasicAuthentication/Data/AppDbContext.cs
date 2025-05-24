using BasicAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicAuthentication.Data
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<TodoTask> TodoTasks { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

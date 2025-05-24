using BasicAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicAuthentication.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoTask> TodoTasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

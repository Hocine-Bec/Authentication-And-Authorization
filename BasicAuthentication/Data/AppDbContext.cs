using BasicAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicAuthentication.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

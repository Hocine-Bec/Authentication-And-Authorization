using BasicAuthentication.Entities;
using BasicAuthentication.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasicAuthentication.Data.Configs
{
    public class TasksConfigurations : IEntityTypeConfiguration<TodoTask>
    {
        public void Configure(EntityTypeBuilder<TodoTask> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Title)
                .HasColumnName("Title")
                .HasColumnType("VARCHAR")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Description)
              .HasColumnName("Description")
              .HasColumnType("VARCHAR")
              .HasMaxLength(300)
              .IsRequired(false);

            builder.Property(x => x.TaskStatus)
                .HasConversion<string>()
                .HasColumnType("VARCHAR")
                .HasMaxLength(30);

            builder.OwnsOne(x => x.DateRange, ts =>
            {
                ts.Property(x => x.StartDate).HasColumnType("date").HasColumnName("StartDate").IsRequired();
                ts.Property(x => x.EndDate).HasColumnType("date").HasColumnName("EndDate").IsRequired();
            });

            builder.HasOne(x => x.User)
               .WithMany(x => x.AssignedTasks)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable("Tasks");
        }
    }
}

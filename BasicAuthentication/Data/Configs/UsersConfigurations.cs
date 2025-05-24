using BasicAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasicAuthentication.Data.Configs
{
    public class UsersConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Username)
                .HasColumnName("Username")
                .HasColumnType("VARCHAR")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Password)
              .HasColumnName("Password")
              .HasColumnType("VARCHAR")
              .HasMaxLength(200)
              .IsRequired();

            builder.Property(x => x.Email)
               .HasColumnName("Email")
               .HasColumnType("VARCHAR")
               .HasMaxLength(100)
               .IsRequired(false);

            builder.ToTable("Users");
        }
    }
}

using BasicAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasicAuthentication.Data.Configs
{
    public class UserPermissionsConfigurations : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.HasKey(x => new { x.UserId, x.Permission });

            builder.HasOne(x => x.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("UserPermissions");
        }
    }
}

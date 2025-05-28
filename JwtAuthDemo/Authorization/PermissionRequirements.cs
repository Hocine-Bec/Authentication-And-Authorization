using JwtAuthDemo.Enums;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDemo.Authorization
{
    public class PermissionRequirements : IAuthorizationRequirement
    {
        public Permission RequiredPermission { get; }

        public PermissionRequirements(Permission permission)
        {
            RequiredPermission = permission;
        }
    }
}

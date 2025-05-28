using BasicAuthentication.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BasicAuthentication.Authorization
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

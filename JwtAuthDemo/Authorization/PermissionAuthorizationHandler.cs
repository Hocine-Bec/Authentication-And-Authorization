using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JwtAuthDemo.Authorization
{
    public class PermissionAuthorizationHandler(AppDbContext context) : AuthorizationHandler<PermissionRequirements>
    {
        private readonly AppDbContext _context = context;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirements requirement)
        {
            if(context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            if(!int.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return;

            var hasPermission = await _context.UserPermissions
                .AnyAsync(x => x.UserId == userId && x.Permission == requirement.RequiredPermission);

            if (!hasPermission)
                return;

            context.Succeed(requirement);
        }
    }
}

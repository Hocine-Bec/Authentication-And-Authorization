using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JwtAuthDemo.Authorization
{
    public class PermissionBasedAuthorizationFilter(AppDbContext context) : IAsyncAuthorizationFilter
    {
        private readonly AppDbContext _context = context;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var attribute = (CheckPermissionAttribute?)context.ActionDescriptor
                .EndpointMetadata
                .FirstOrDefault(x => x is CheckPermissionAttribute);

            if (attribute != null)
            {
                var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;

                if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var userIdClaims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if(userIdClaims == null || !int.TryParse(userIdClaims.Value, out var userId))
                {
                    context.Result = new ForbidResult();
                    return;
                }    

                var hasPermission = await _context.UserPermissions.AnyAsync(
                    x => x.UserId == userId && attribute.Permission == x.Permission);

                if (!hasPermission)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}

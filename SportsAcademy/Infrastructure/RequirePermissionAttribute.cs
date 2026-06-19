using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SportsAcademy.Services;
using System.Security.Claims;

namespace SportsAcademy.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePermissionAttribute(string module, string action = "View") : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Account",
                new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        var roleIdClaim = user.FindFirst("RoleId")?.Value;
        if (!int.TryParse(roleIdClaim, out var roleId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var permService = context.HttpContext.RequestServices.GetRequiredService<PermissionService>();
        var allowed = await permService.HasPermissionAsync(roleId, module, action);

        if (!allowed)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}

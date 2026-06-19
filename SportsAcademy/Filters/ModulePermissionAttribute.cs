using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SportsAcademy.Services;

namespace SportsAcademy.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ModulePermissionAttribute : Attribute, IFilterFactory
{
    public string ModuleName { get; }
    public bool IsReusable => false;

    public ModulePermissionAttribute(string moduleName) => ModuleName = moduleName;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var permService = serviceProvider.GetRequiredService<IPermissionService>();
        return new ModulePermissionFilter(ModuleName, permService);
    }
}

public class ModulePermissionFilter : IAsyncActionFilter
{
    private readonly string _moduleName;
    private readonly IPermissionService _permissionService;

    public ModulePermissionFilter(string moduleName, IPermissionService permissionService)
    {
        _moduleName = moduleName;
        _permissionService = permissionService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Login", "Account",
                new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        // Admin-only modules bypass the permission table
        if (_moduleName is "UserManagement" or "RoleManagement")
        {
            if (user.FindFirst("RoleName")?.Value != "SuperAdmin")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }
            await next();
            return;
        }

        if (!int.TryParse(user.FindFirst("RoleId")?.Value, out var roleId))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var actionName = context.RouteData.Values["action"]?.ToString() ?? "";
        var required = ResolvePermission(actionName);

        if (!await _permissionService.HasPermissionAsync(roleId, _moduleName, required))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            return;
        }

        await next();
    }

    private static string ResolvePermission(string action) => action.ToLower() switch
    {
        "create" => "Create",
        "edit" or "update" => "Edit",
        "delete" or "deleteconfirmed" => "Delete",
        _ => "View"
    };
}

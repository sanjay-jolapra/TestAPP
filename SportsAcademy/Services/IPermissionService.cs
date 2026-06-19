namespace SportsAcademy.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int roleId, string moduleName, string permission = "View");
    Task InvalidateCacheAsync(int roleId);
}

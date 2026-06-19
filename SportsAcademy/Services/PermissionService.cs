using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsAcademy.Data;
using SportsAcademy.Models.Auth;

namespace SportsAcademy.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public PermissionService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(int roleId, string moduleName, string permission = "View")
    {
        var cacheKey = $"perms_{roleId}";
        if (!_cache.TryGetValue(cacheKey, out Dictionary<string, RolePermission>? perms) || perms == null)
        {
            perms = await _db.RolePermissions
                .Include(rp => rp.Module)
                .Where(rp => rp.RoleId == roleId)
                .ToDictionaryAsync(rp => rp.Module.Name);
            _cache.Set(cacheKey, perms, TimeSpan.FromMinutes(10));
        }

        if (!perms.TryGetValue(moduleName, out var perm)) return false;

        return permission switch
        {
            "View"   => perm.CanView,
            "Create" => perm.CanCreate,
            "Edit"   => perm.CanEdit,
            "Delete" => perm.CanDelete,
            _        => false
        };
    }

    public Task InvalidateCacheAsync(int roleId)
    {
        _cache.Remove($"perms_{roleId}");
        return Task.CompletedTask;
    }
}

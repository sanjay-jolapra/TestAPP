using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Services;

public class PermissionService
{
    private readonly ApplicationDbContext _db;

    public PermissionService(ApplicationDbContext db) => _db = db;

    public async Task<RolePermission?> GetAsync(int roleId, string module)
        => await _db.RolePermissions
            .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Module == module);

    public async Task<bool> HasPermissionAsync(int roleId, string module, string action)
    {
        var perm = await GetAsync(roleId, module);
        if (perm == null) return false;
        return action switch
        {
            "View"   => perm.CanView,
            "Create" => perm.CanCreate,
            "Edit"   => perm.CanEdit,
            "Delete" => perm.CanDelete,
            _        => false
        };
    }

    public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId)
        => await _db.RolePermissions.Where(p => p.RoleId == roleId).ToListAsync();
}

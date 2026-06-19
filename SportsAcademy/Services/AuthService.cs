using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models.Auth;

namespace SportsAcademy.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db) => _db = db;

    public async Task<AppUser?> ValidateUserAsync(string username, string password)
    {
        var user = await _db.AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) return null;
        return HashPassword(password, user.PasswordSalt) == user.PasswordHash ? user : null;
    }

    public async Task<AppUser?> GetUserByIdAsync(int id) =>
        await _db.AppUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

    public string GenerateSalt()
    {
        var bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _db.AppUsers.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}

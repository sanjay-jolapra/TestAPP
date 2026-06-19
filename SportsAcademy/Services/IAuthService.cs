using SportsAcademy.Models.Auth;

namespace SportsAcademy.Services;

public interface IAuthService
{
    Task<AppUser?> ValidateUserAsync(string username, string password);
    Task<AppUser?> GetUserByIdAsync(int id);
    string HashPassword(string password, string salt);
    string GenerateSalt();
    Task UpdateLastLoginAsync(int userId);
}

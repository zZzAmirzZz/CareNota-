// IAdminRepository.cs
using CareNota.Models;
using Microsoft.AspNetCore.Identity;

namespace CareNota.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<Admin?> GetByUserIdAsync(string UserId);
    Task<ApplicationUser?> GetUserByIdAsync(string UserId);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser User);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser User, string CurrentPassword, string NewPassword);
    Task SaveAsync();
    Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<IdentityResult> DeleteAsync(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
}
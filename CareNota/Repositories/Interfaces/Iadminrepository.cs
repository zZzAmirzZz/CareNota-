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
}
// AdminRepository.cs
using CareNota.Repositories.Interfaces;
using CareNota.Data;
using CareNota.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _Context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminRepository(
        ApplicationDbContext Context,
        UserManager<ApplicationUser> UserManager)
    {
        _Context = Context;
        _userManager = UserManager;
    }

    public async Task<Admin?> GetByUserIdAsync(string UserId)
        => await _Context.Admins
                         .Include(A => A.User)
                         .FirstOrDefaultAsync(A => A.UserId == UserId);
    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.AddToRoleAsync(user, role);
    }
    public async Task<ApplicationUser?> GetUserByIdAsync(string UserId)
        => await _userManager.FindByIdAsync(UserId);

    // ✅ Now actually used by the service
    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser User)
        => await _userManager.UpdateAsync(User);

    // ✅ New — keeps UserManager out of the service layer
    public async Task<IdentityResult> ChangePasswordAsync(
        ApplicationUser User, string CurrentPassword, string NewPassword)
        => await _userManager.ChangePasswordAsync(User, CurrentPassword, NewPassword);

    public async Task SaveAsync()
        => await _Context.SaveChangesAsync();
    public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role)
    {
        return await _userManager.GetUsersInRoleAsync(role);
    }

    public async Task<ApplicationUser?> FindByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }
}
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
    private readonly UserManager<ApplicationUser> _UserManager;

    public AdminRepository(
        ApplicationDbContext Context,
        UserManager<ApplicationUser> UserManager)
    {
        _Context = Context;
        _UserManager = UserManager;
    }

    public async Task<Admin?> GetByUserIdAsync(string UserId)
        => await _Context.Admins
                         .Include(A => A.User)
                         .FirstOrDefaultAsync(A => A.UserId == UserId);

    public async Task<ApplicationUser?> GetUserByIdAsync(string UserId)
        => await _UserManager.FindByIdAsync(UserId);

    // ✅ Now actually used by the service
    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser User)
        => await _UserManager.UpdateAsync(User);

    // ✅ New — keeps UserManager out of the service layer
    public async Task<IdentityResult> ChangePasswordAsync(
        ApplicationUser User, string CurrentPassword, string NewPassword)
        => await _UserManager.ChangePasswordAsync(User, CurrentPassword, NewPassword);

    public async Task SaveAsync()
        => await _Context.SaveChangesAsync();
}
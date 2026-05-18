// AdminService.cs
using AutoMapper;
using CareNota.DTOs.Admin;
using CareNota.Services.Interfaces;
using CareNota.Repositories.Interfaces;
using CareNota.Models;

namespace CareNota.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _AdminRepository;
    private readonly IMapper _Mapper;

    // ✅ UserManager removed — repository handles all Identity calls
    public AdminService(
        IAdminRepository AdminRepository,
        IMapper Mapper)
    {
        _AdminRepository = AdminRepository;
        _Mapper = Mapper;
    }

    // ── Get Profile ──────────────────────────────────────────────────────────

    public async Task<AdminProfileDto?> GetProfileAsync(string UserId)
    {
        var User = await _AdminRepository.GetUserByIdAsync(UserId);
        if (User is null) return null;

        return _Mapper.Map<AdminProfileDto>(User);
    }

    // ── Update Profile ────────────────────────────────────────────────────────

    public async Task<bool> UpdateProfileAsync(string UserId, UpdateAdminProfileDto Dto)
    {
        var User = await _AdminRepository.GetUserByIdAsync(UserId);
        if (User is null) return false;

        User.FullName = Dto.FullName;
        User.PhoneNumber = Dto.PhoneNumber;
        User.Gender = Dto.Gender;

        // ✅ Now goes through the repository, not UserManager directly
        var Result = await _AdminRepository.UpdateUserAsync(User);
        return Result.Succeeded;
    }

    // ── Change Password ───────────────────────────────────────────────────────

    public async Task<(bool Success, string Error)> ChangePasswordAsync(
        string UserId, ChangePasswordDto Dto)
    {
        if (Dto.NewPassword != Dto.ConfirmNewPassword)
            return (false, "New password and confirmation do not match.");

        var User = await _AdminRepository.GetUserByIdAsync(UserId);
        if (User is null)
            return (false, "User not found.");

        // ✅ Now goes through the repository, not UserManager directly
        var Result = await _AdminRepository.ChangePasswordAsync(
            User, Dto.CurrentPassword, Dto.NewPassword);

        if (!Result.Succeeded)
        {
            var Errors = string.Join(" | ", Result.Errors.Select(E => E.Description));
            return (false, Errors);
        }

        return (true, string.Empty);
    }
}
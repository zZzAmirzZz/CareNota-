// AdminService.cs
using AutoMapper;
using CareNota.DTOs;
using CareNota.DTOs.Admin;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using CareNota.Services.Interfaces;
namespace CareNota.Services;
using CareNota.Data;
using Microsoft.AspNetCore.Identity;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _AdminRepository;
    private readonly IMapper _Mapper;
    private readonly ApplicationDbContext _context; // only if you have a Doctor/Receptionist table

    // ✅ UserManager removed — repository handles all Identity calls
    public AdminService(
        IAdminRepository AdminRepository,
        IMapper Mapper,
        ApplicationDbContext Context)
    {
        _AdminRepository = AdminRepository;
        _Mapper = Mapper;
        _context = Context;
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
    public async Task<IEnumerable<ReceptionistProfileDto>> GetAllReceptionistsAsync()
    {
        var receptionists = await _AdminRepository.GetUsersInRoleAsync("receptionist");

        return receptionists.Select(u => new ReceptionistProfileDto
        {
            Id = u.Id,
            FullName = u.FullName,   // adjust to your ApplicationUser property name
            Email = u.Email,
            PhoneNumber = u.PhoneNumber
        });
    }

    public async Task<bool> DeleteReceptionistAsync(string id)
    {
        var user = await _AdminRepository.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _AdminRepository.DeleteAsync(user);
        return result.Succeeded;
    }
    public async Task<AccountCreatedResponseDto> CreateDoctorAccountAsync(CreateDoctorDto dto)
    {
        // 1. Check if email already exists
        var existingUser = await _AdminRepository.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        // 2. Create the ApplicationUser
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true // Admin-created accounts are pre-confirmed
        };
        var result = await _AdminRepository.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        // 3. Assign the Doctor role
        await _AdminRepository.AddToRoleAsync(user, "doctor");

        // 4. Create the linked Doctor record
        var doctor = new Doctor
        {
            UserId = user.Id,
            Specialty = dto.Specialty,
        };
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return new AccountCreatedResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = "doctor",
            Message = "Doctor account created successfully."
        };
    }
    public async Task<AccountCreatedResponseDto> CreateReceptionistAccountAsync(CreateReceptionistDto dto)
    {
        // 1. Check if email already exists
        var existingUser = await _AdminRepository.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        // 2. Create the ApplicationUser
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _AdminRepository.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        // 3. Assign the Receptionist role
        await _AdminRepository.AddToRoleAsync(user, "receptionist");

        // 4. Create the linked Receptionist record
        var receptionist = new Receptionist
        {
            UserId = user.Id,
        };
        _context.Receptionists.Add(receptionist);
        await _context.SaveChangesAsync();

        return new AccountCreatedResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = "receptionist",
            Message = "Receptionist account created successfully."
        };
    }
}
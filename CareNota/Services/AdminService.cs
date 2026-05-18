using CareNota.Data;
using CareNota.DTOs.Admin;
using CareNota.Models;
using Microsoft.AspNetCore.Identity;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context; // only if you have a Doctor/Receptionist table

    public AdminService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<AccountCreatedResponseDto> CreateDoctorAccountAsync(CreateDoctorDto dto)
    {
        // 1. Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
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

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        // 3. Assign the Doctor role
        await _userManager.AddToRoleAsync(user, "doctor");

        // 4. Create the linked Doctor record
        var doctor = new Doctor
        {
            UserId = user.Id,
            Specialty = dto.Specialization,
           
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
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "receptionist");

        var receptionist = new Receptionist
        {
            UserId = user.Id
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
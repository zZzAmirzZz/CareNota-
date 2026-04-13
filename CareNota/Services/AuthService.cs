using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CareNota.Data;
using CareNota.DTOs.Auth;
using CareNota.Services.Interfaces;
using CareNota.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CareNota.Services;

public class AuthService(
    UserManager<ApplicationUser> UserManager,
    IConfiguration Configuration,
    ApplicationDbContext Context) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto Dto)
    {
        // Validate role
        string[] AllowedRoles = [RoleSeeder.Doctor, RoleSeeder.Patient, RoleSeeder.Receptionist];
        if (!AllowedRoles.Contains(Dto.Role))
            throw new ArgumentException($"Invalid role. Allowed: {string.Join(", ", AllowedRoles)}");

        // Check duplicate email
        if (await UserManager.FindByEmailAsync(Dto.Email) is not null)
            throw new InvalidOperationException("Email is already registered.");

        var User = new ApplicationUser
        {
            FullName = Dto.FullName,
            Email = Dto.Email,
            UserName = Dto.Email,
            PhoneNumber = Dto.PhoneNumber,
            Gender = Dto.Gender
        };

        var CreateResult = await UserManager.CreateAsync(User, Dto.Password);
        if (!CreateResult.Succeeded)
        {
            var Errors = string.Join("; ", CreateResult.Errors.Select(E => E.Description));
            throw new InvalidOperationException(Errors);
        }

        await UserManager.AddToRoleAsync(User, Dto.Role);

        // Seed role-specific profile row
        await CreateRoleProfileAsync(User.Id, Dto.Role);

        return await BuildAuthResponseAsync(User);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto Dto)
    {
        var User = await UserManager.FindByEmailAsync(Dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await UserManager.CheckPasswordAsync(User, Dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await BuildAuthResponseAsync(User);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto Dto)
    {
        var Principal = GetPrincipalFromExpiredToken(Dto.AccessToken);
        var UserId = Principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Invalid access token.");

        var User = await UserManager.FindByIdAsync(UserId)
            ?? throw new UnauthorizedAccessException("User not found.");

        if (User.RefreshToken != Dto.RefreshToken ||
            User.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");

        return await BuildAuthResponseAsync(User);
    }

    public async Task RevokeTokenAsync(string UserId)
    {
        var User = await UserManager.FindByIdAsync(UserId)
            ?? throw new KeyNotFoundException("User not found.");

        User.RefreshToken = null;
        User.RefreshTokenExpiryTime = null;
        await UserManager.UpdateAsync(User);
    }

    // ─── Private Helpers ────────────────────────────────────────────────────────

    private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser User)
    {
        var Roles = await UserManager.GetRolesAsync(User);
        var AccessToken = GenerateAccessToken(User, Roles);
        var NewRefreshToken = GenerateRefreshToken();

        User.RefreshToken = NewRefreshToken;
        User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            int.Parse(Configuration["Jwt:RefreshTokenExpiryDays"]!));
        await UserManager.UpdateAsync(User);

        return new AuthResponseDto
        {
            AccessToken = AccessToken,
            RefreshToken = NewRefreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(Configuration["Jwt:ExpiryMinutes"]!)),
            UserId = User.Id,
            Email = User.Email!,
            FullName = User.FullName,
            Roles = Roles
        };
    }

    private string GenerateAccessToken(ApplicationUser User, IList<string> Roles)
    {
        var Claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, User.Id),
            new(ClaimTypes.Email, User.Email!),
            new(ClaimTypes.Name, User.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        Claims.AddRange(Roles.Select(R => new Claim(ClaimTypes.Role, R)));

        var Key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!));
        var Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        var Token = new JwtSecurityToken(
            issuer: Configuration["Jwt:Issuer"],
            audience: Configuration["Jwt:Audience"],
            claims: Claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(Configuration["Jwt:ExpiryMinutes"]!)),
            signingCredentials: Credentials);

        return new JwtSecurityTokenHandler().WriteToken(Token);
    }

    private static string GenerateRefreshToken()
    {
        var RandomBytes = new byte[64];
        using var Rng = RandomNumberGenerator.Create();
        Rng.GetBytes(RandomBytes);
        return Convert.ToBase64String(RandomBytes);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string Token)
    {
        var TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // Allow expired tokens for refresh
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
        };

        var Handler = new JwtSecurityTokenHandler();
        var Principal = Handler.ValidateToken(Token, TokenValidationParameters, out var SecurityToken);

        if (SecurityToken is not JwtSecurityToken JwtToken ||
            !JwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenException("Invalid token.");

        return Principal;
    }

    private async Task CreateRoleProfileAsync(string UserId, string Role)
    {
        switch (Role)
        {
            case "Doctor":
                Context.Doctors.Add(new Doctor { UserId = UserId });
                break;
            case "Patient":
                Context.Patients.Add(new Patient { UserId = UserId });
                break;
            case "Receptionist":
                Context.Receptionists.Add(new Receptionist { UserId = UserId });
                break;
        }
        await Context.SaveChangesAsync();
    }
}
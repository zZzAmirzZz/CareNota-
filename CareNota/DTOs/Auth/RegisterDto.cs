using System.ComponentModel.DataAnnotations;

namespace CareNota.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    // ✅ Patient profile fields — all optional at registration
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? InsuranceInfo { get; set; }
    public string? ChronicConditions { get; set; }
}

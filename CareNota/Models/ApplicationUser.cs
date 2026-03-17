using Microsoft.AspNetCore.Identity;

namespace CareNota.Models;

public class ApplicationUser : IdentityUser
{
    // Maps to your old FirstName + LastName
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    // Role is handled by Identity — no need to store it manually
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Refresh Token support
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation (1-to-1 with each profile table)
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Receptionist? Receptionist { get; set; }
    //Email, PhoneNumber, PasswordHash come free from IdentityUser — don't redeclare them.


}
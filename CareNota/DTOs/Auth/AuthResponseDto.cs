public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];

    // Role-specific IDs — only one will be populated per login
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public int? ReceptionistId { get; set; }
    public string? AdminId { get; set; }  // Admin uses string (Identity Id)
}
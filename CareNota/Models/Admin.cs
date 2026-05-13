namespace CareNota.Models;

public class Admin
{
    public int AdminId { get; set; }
    public bool IsFirstLogin { get; set; } = true;

    // FK → ApplicationUser (Identity uses string for UserId)
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser? User { get; set; }
}
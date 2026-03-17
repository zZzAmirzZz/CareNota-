using Microsoft.AspNetCore.Identity;

namespace CareNota.Data;

public static class RoleSeeder
{
    public static readonly string Doctor = "Doctor";
    public static readonly string Patient = "Patient";
    public static readonly string Receptionist = "Receptionist";

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> RoleManager)
    {
        string[] Roles = [Doctor, Patient, Receptionist];

        foreach (string Role in Roles)
        {
            if (!await RoleManager.RoleExistsAsync(Role))
                await RoleManager.CreateAsync(new IdentityRole(Role));
        }
    }
}
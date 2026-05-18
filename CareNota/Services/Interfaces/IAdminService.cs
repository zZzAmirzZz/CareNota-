using CareNota.DTOs.Admin;

namespace CareNota.Services.Interfaces;

public interface IAdminService
{
    Task<AdminProfileDto?> GetProfileAsync(string UserId);
    Task<bool> UpdateProfileAsync(string UserId, UpdateAdminProfileDto Dto);
    Task<(bool Success, string Error)> ChangePasswordAsync(string UserId, ChangePasswordDto Dto);
}
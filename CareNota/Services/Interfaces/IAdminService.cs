using CareNota.DTOs;
using CareNota.DTOs.Admin;

namespace CareNota.Services.Interfaces;

public interface IAdminService
{
    Task<AdminProfileDto?> GetProfileAsync(string UserId);
    Task<bool> UpdateProfileAsync(string UserId, UpdateAdminProfileDto Dto);
    Task<(bool Success, string Error)> ChangePasswordAsync(string UserId, ChangePasswordDto Dto);
    Task<IEnumerable<ReceptionistProfileDto>> GetAllReceptionistsAsync();
    Task<bool> DeleteReceptionistAsync(string id);
    Task<AccountCreatedResponseDto> CreateDoctorAccountAsync(CreateDoctorDto dto);
    Task<AccountCreatedResponseDto> CreateReceptionistAccountAsync(CreateReceptionistDto dto);
    }

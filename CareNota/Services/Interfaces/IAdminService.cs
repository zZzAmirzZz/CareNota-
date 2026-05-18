using CareNota.DTOs.Admin;

public interface IAdminService
{
    Task<AccountCreatedResponseDto> CreateDoctorAccountAsync(CreateDoctorDto dto);
    Task<AccountCreatedResponseDto> CreateReceptionistAccountAsync(CreateReceptionistDto dto);
}
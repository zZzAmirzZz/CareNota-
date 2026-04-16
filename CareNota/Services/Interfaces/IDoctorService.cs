using CareNota.DTOs.Doctor;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllAsync();

    Task<DoctorDto?> GetByIdAsync(int DoctorId);

    // Partial keyword match on Specialty field
    Task<IEnumerable<DoctorDto>> GetBySpecialtyAsync(string Specialty);

    // Doctors can only update their Specialty (name/email live on ApplicationUser)
    Task<DoctorDto> UpdateAsync(int DoctorId, UpdateDoctorDto Dto);

    Task DeleteAsync(int DoctorId);
}

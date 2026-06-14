using CareNota.Models;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task<Doctor?> GetByIdAsync(int doctorId);
    Task<IEnumerable<Doctor>> GetBySpecialtyAsync(string specialty);
    Task<Doctor?> GetByUserIdAsync(string userId);
}
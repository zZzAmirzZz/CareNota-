using CareNota.Models;

public interface IDoctorRepository : IRepository<Doctor>
{
    // Get doctor row by the linked ApplicationUser ID (string GUID)
    Task<Doctor?> GetByUserIdAsync(string UserId);

    // Filter doctors by specialty keyword (partial match)
    Task<IEnumerable<Doctor>> GetBySpecialtyAsync(string Specialty);
}
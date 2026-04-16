using CareNota.Models;

public interface IPatientRepository : IRepository<Patient>
{
    // Get patient row by the linked ApplicationUser ID (string GUID)
    Task<Patient?> GetByUserIdAsync(string UserId);

    // Loads Patient + MedicalHistory navigation
    Task<Patient?> GetWithMedicalHistoryAsync(int PatientId);

    // Loads Patient + Appointments navigation
    Task<Patient?> GetWithAppointmentsAsync(int PatientId);

    // Loads Patient + Reminders navigation
    Task<Patient?> GetWithRemindersAsync(int PatientId);

    // Full-text search on FirstName + LastName through ApplicationUser
    Task<IEnumerable<Patient>> SearchByNameAsync(string Name);
}
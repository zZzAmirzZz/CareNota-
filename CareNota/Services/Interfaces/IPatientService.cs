using CareNota.DTOs.Patient;

public interface IPatientService
{
    // Returns all patients (flat list — no heavy navigation)
    Task<IEnumerable<PatientDto>> GetAllAsync();

    // Single patient by their PatientID (flat)
    Task<PatientDto?> GetByIdAsync(int PatientId);

    // Single patient with MedicalHistory + Appointments included
    Task<PatientDetailDto?> GetDetailsAsync(int PatientId);

    // Full-text search on patient full name
    Task<IEnumerable<PatientDto>> SearchByNameAsync(string Name);

    // Update only the patient-owned fields (Gender, BloodType, Allergies, InsuranceInfo)
    Task<PatientDto> UpdateAsync(int PatientId, UpdatePatientDto Dto);

    // Hard delete — cascades to MedicalHistory, Appointments, Reminders
    Task DeleteAsync(int PatientId);
}
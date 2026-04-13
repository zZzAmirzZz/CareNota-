using CareNota.DTOs.Diagnosis;
using CareNota.DTOs.LabTest;
using CareNota.DTOs.Medication;
using CareNota.DTOs.Prescription;
using CareNota.DTOs.Visit;

namespace CareNota.Services.Interfaces;

// ══════════════════════════════════════════════════════════════════════════════
// IVisitService
// ══════════════════════════════════════════════════════════════════════════════
public interface IVisitService
{
    Task<IEnumerable<VisitDto>> GetAllAsync();
    Task<VisitDto?> GetByIdAsync(int VisitId);
    Task<VisitDetailDto?> GetDetailsAsync(int VisitId);
    Task<IEnumerable<VisitDto>> GetByPatientIdAsync(int PatientId);
    Task<VisitDto?> GetByAppointmentIdAsync(int AppointmentId);
    Task<VisitDto> CreateAsync(CreateVisitDto Dto);
    Task<VisitDto> UpdateAsync(int VisitId, UpdateVisitDto Dto);
    Task DeleteAsync(int VisitId);
}

// ══════════════════════════════════════════════════════════════════════════════
// IDiagnosisService
// ══════════════════════════════════════════════════════════════════════════════
public interface IDiagnosisService
{
    Task<IEnumerable<DiagnosisDto>> GetAllAsync();
    Task<DiagnosisDto?> GetByIcdCodeAsync(string ICD10Code);
    Task<IEnumerable<DiagnosisDto>> SearchAsync(string Query);
    Task<IEnumerable<DiagnosisDto>> GetByVisitIdAsync(int VisitId);
    Task<DiagnosisDto> CreateAsync(CreateDiagnosisDto Dto);
    Task AssignToVisitAsync(int VisitId, AssignDiagnosisToVisitDto Dto);
    Task RemoveFromVisitAsync(int VisitId, string ICD10Code);
    Task DeleteAsync(string ICD10Code);
}

// ══════════════════════════════════════════════════════════════════════════════
// IPrescriptionService
// ══════════════════════════════════════════════════════════════════════════════
public interface IPrescriptionService
{
    Task<PrescriptionDto?> GetByIdAsync(int PrescriptionId);
    Task<PrescriptionDto?> GetByVisitIdAsync(int VisitId);
    Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto Dto);
    Task<PrescriptionDto> UpdateAsync(int PrescriptionId, UpdatePrescriptionDto Dto);
    Task<PrescriptionDto> AddMedicationAsync(int PrescriptionId, AddMedicationToPrescriptionDto Dto);
    Task<PrescriptionDto> RemoveMedicationAsync(int PrescriptionId, int MedicationId);
    Task DeleteAsync(int PrescriptionId);
}

// ══════════════════════════════════════════════════════════════════════════════
// IMedicationService
// ══════════════════════════════════════════════════════════════════════════════
public interface IMedicationService
{
    Task<IEnumerable<MedicationDto>> GetAllAsync();
    Task<MedicationDto?> GetByIdAsync(int MedicationId);
    Task<IEnumerable<MedicationDto>> SearchByNameAsync(string Name);
    Task<IEnumerable<MedicationDto>> GetByTypeAsync(string Type);
    Task<MedicationDto> CreateAsync(CreateMedicationDto Dto);
    Task<MedicationDto> UpdateAsync(int MedicationId, UpdateMedicationDto Dto);
    Task DeleteAsync(int MedicationId);
}

// ══════════════════════════════════════════════════════════════════════════════
// ILabTestService
// ══════════════════════════════════════════════════════════════════════════════
public interface ILabTestService
{
    Task<IEnumerable<LabTestDto>> GetByVisitIdAsync(int VisitId);
    Task<LabTestDto?> GetByIdAsync(int LabTestId);
    Task<LabTestDto> CreateAsync(CreateLabTestDto Dto);
    Task<LabTestDto> UploadResultAsync(int LabTestId, IFormFile File, string UploadFolder);
    Task DeleteAsync(int LabTestId);
}
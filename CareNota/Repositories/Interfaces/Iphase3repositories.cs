using CareNota.Models;

namespace CareNota.Repositories.Interfaces;

// ══════════════════════════════════════════════════════════════════════════════
// IVisitRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface IVisitRepository : IRepository<Visit>
{
    Task<Visit?> GetByIdWithDetailsAsync(int VisitId);
    Task<Visit?> GetByAppointmentIdAsync(int AppointmentId);
    Task<IEnumerable<Visit>> GetByPatientIdAsync(int PatientId);
    Task<Visit?> GetWithDiagnosesAsync(int VisitId);
    Task<Visit?> GetWithPrescriptionAsync(int VisitId);
    Task<Visit?> GetWithLabTestsAsync(int VisitId);
}

// ══════════════════════════════════════════════════════════════════════════════
// IDiagnosisRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface IDiagnosisRepository : IRepository<Diagnosis>
{
    Task<Diagnosis?> GetByIcdCodeAsync(string ICD10Code);
    Task<IEnumerable<Diagnosis>> SearchByNameAsync(string Name);
    Task<IEnumerable<Diagnosis>> GetByVisitIdAsync(int VisitId);
    Task<bool> IcdCodeExistsAsync(string ICD10Code);
    Task AddVisitDiagnosisAsync(VisitDiagnosis VisitDiagnosis);
    Task RemoveVisitDiagnosisAsync(int VisitId, string ICD10Code);
    Task<bool> VisitDiagnosisExistsAsync(int VisitId, string ICD10Code);
}

// ══════════════════════════════════════════════════════════════════════════════
// IPrescriptionRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<Prescription?> GetByVisitIdAsync(int VisitId);
    Task<Prescription?> GetWithMedicationsAsync(int PrescriptionId);
    Task<Prescription?> GetFullDetailsAsync(int PrescriptionId);
    Task AddMedicationAsync(PrescriptionMedication Pm);
    Task RemoveMedicationAsync(int PrescriptionId, int MedicationId);
    Task UpdateMedicationAsync(PrescriptionMedication Pm);
    Task<PrescriptionMedication?> GetPrescriptionMedicationAsync(int PrescriptionId, int MedicationId);
}

// ══════════════════════════════════════════════════════════════════════════════
// IMedicationRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface IMedicationRepository : IRepository<Medication>
{
    Task<IEnumerable<Medication>> SearchByNameAsync(string Name);
    Task<IEnumerable<Medication>> GetByTypeAsync(string MedicationType);
    Task<bool> NameExistsAsync(string MedicationName);
}

// ══════════════════════════════════════════════════════════════════════════════
// ILabTestRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface ILabTestRepository : IRepository<LabTest>
{
    Task<IEnumerable<LabTest>> GetByVisitIdAsync(int VisitId);
    Task<LabTest?> GetByIdWithVisitAsync(int LabTestId);
    Task<bool> UpdateResultUrlAsync(int LabTestId, string ResultUrl);
}
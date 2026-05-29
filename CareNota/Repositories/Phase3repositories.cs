using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Repositories;

// ══════════════════════════════════════════════════════════════════════════════
// VisitRepository
// ══════════════════════════════════════════════════════════════════════════════
public class VisitRepository : GenericRepository<Visit>, IVisitRepository
{
    public VisitRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<Visit?> GetByIdWithDetailsAsync(int VisitId)
        => await DbSet
            .Include(V => V.Appointment)
                .ThenInclude(A => A.Patient)
                    .ThenInclude(P => P.User)
            .Include(V => V.Appointment)
                .ThenInclude(A => A.Receptionist)
            .Include(V => V.VisitDiagnoses)
                .ThenInclude(VD => VD.Diagnosis)
            .Include(V => V.Prescription)
                .ThenInclude(P => P!.PrescriptionMedications)
                    .ThenInclude(PM => PM.Medication)
            .Include(V => V.LabTests)
            .Include(V => V.AudioRecord)
            .Include(V => V.AISummaries)
            .FirstOrDefaultAsync(V => V.VisitID == VisitId);

    public async Task<Visit?> GetByAppointmentIdAsync(int AppointmentId)
        => await DbSet
            .Include(V => V.Appointment)
                .ThenInclude(A => A.Patient)
                    .ThenInclude(P => P.User)
            .FirstOrDefaultAsync(V => V.AppointmentID == AppointmentId);

    public async Task<IEnumerable<Visit>> GetByPatientIdAsync(int PatientId)
        => await DbSet
            .Include(V => V.Appointment)
                .ThenInclude(A => A.Patient)
                    .ThenInclude(P => P.User)
            .Include(V => V.VisitDiagnoses)
                .ThenInclude(VD => VD.Diagnosis)
            .Where(V => V.Appointment.PatientID == PatientId)
            .OrderByDescending(V => V.VisitDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Visit?> GetWithDiagnosesAsync(int VisitId)
        => await DbSet
            .Include(V => V.VisitDiagnoses)
                .ThenInclude(VD => VD.Diagnosis)
            .FirstOrDefaultAsync(V => V.VisitID == VisitId);

    public async Task<Visit?> GetWithPrescriptionAsync(int VisitId)
        => await DbSet
            .Include(V => V.Prescription)
                .ThenInclude(P => P!.PrescriptionMedications)
                    .ThenInclude(PM => PM.Medication)
            .FirstOrDefaultAsync(V => V.VisitID == VisitId);

    public async Task<Visit?> GetWithLabTestsAsync(int VisitId)
        => await DbSet
            .Include(V => V.LabTests)
            .FirstOrDefaultAsync(V => V.VisitID == VisitId);
}

// ══════════════════════════════════════════════════════════════════════════════
// DiagnosisRepository
// ══════════════════════════════════════════════════════════════════════════════
public class DiagnosisRepository : GenericRepository<Diagnosis>, IDiagnosisRepository
{
    private readonly DbSet<VisitDiagnosis> _VisitDiagnoses;

    public DiagnosisRepository(ApplicationDbContext Context) : base(Context)
        => _VisitDiagnoses = Context.Set<VisitDiagnosis>();

    public async Task<Diagnosis?> GetByIcdCodeAsync(string ICD10Code)
        => await DbSet.FindAsync(ICD10Code);

    public async Task<IEnumerable<Diagnosis>> SearchByNameAsync(string Name)
        => await DbSet
            .Where(D => D.DiagnosisName.ToLower().Contains(Name.ToLower()) ||
                        D.ICD10Code.ToLower().Contains(Name.ToLower()))
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Diagnosis>> GetByVisitIdAsync(int VisitId)
        => await _VisitDiagnoses
            .Where(VD => VD.VisitID == VisitId)
            .Include(VD => VD.Diagnosis)
            .Select(VD => VD.Diagnosis)
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> IcdCodeExistsAsync(string ICD10Code)
        => await DbSet.AnyAsync(D => D.ICD10Code == ICD10Code);

    public async Task AddVisitDiagnosisAsync(VisitDiagnosis VisitDiagnosis)
        => await _VisitDiagnoses.AddAsync(VisitDiagnosis);

    public async Task RemoveVisitDiagnosisAsync(int VisitId, string ICD10Code)
    {
        var Entity = await _VisitDiagnoses.FindAsync(VisitId, ICD10Code);
        if (Entity is not null)
            _VisitDiagnoses.Remove(Entity);
    }

    public async Task<bool> VisitDiagnosisExistsAsync(int VisitId, string ICD10Code)
        => await _VisitDiagnoses
            .AnyAsync(VD => VD.VisitID == VisitId && VD.ICD10Code == ICD10Code);
}

// ══════════════════════════════════════════════════════════════════════════════
// PrescriptionRepository
// ══════════════════════════════════════════════════════════════════════════════
public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
{
    private readonly DbSet<PrescriptionMedication> _PrescriptionMedications;

    public PrescriptionRepository(ApplicationDbContext Context) : base(Context)
        => _PrescriptionMedications = Context.Set<PrescriptionMedication>();

    public async Task<Prescription?> GetByVisitIdAsync(int VisitId)
        => await DbSet
            .Include(P => P.PrescriptionMedications)
                .ThenInclude(PM => PM.Medication)
            .FirstOrDefaultAsync(P => P.VisitID == VisitId);

    public async Task<Prescription?> GetWithMedicationsAsync(int PrescriptionId)
        => await DbSet
            .Include(P => P.PrescriptionMedications)
                .ThenInclude(PM => PM.Medication)
            .FirstOrDefaultAsync(P => P.PrescriptionID == PrescriptionId);

    public async Task<Prescription?> GetFullDetailsAsync(int PrescriptionId)
        => await DbSet
            .Include(P => P.Visit)
                .ThenInclude(V => V.Appointment)
                    .ThenInclude(A => A.Patient)
                        .ThenInclude(Pt => Pt.User)
            .Include(P => P.PrescriptionMedications)
                .ThenInclude(PM => PM.Medication)
            .Include(P => P.Reminders)
            .FirstOrDefaultAsync(P => P.PrescriptionID == PrescriptionId);

    public async Task AddMedicationAsync(PrescriptionMedication Pm)
        => await _PrescriptionMedications.AddAsync(Pm);

    public async Task RemoveMedicationAsync(int PrescriptionId, int MedicationId)
    {
        var Entity = await _PrescriptionMedications.FindAsync(PrescriptionId, MedicationId);
        if (Entity is not null)
            _PrescriptionMedications.Remove(Entity);
    }

    public Task UpdateMedicationAsync(PrescriptionMedication Pm)
    {
        _PrescriptionMedications.Update(Pm);
        return Task.CompletedTask;
    }

    public async Task<PrescriptionMedication?> GetPrescriptionMedicationAsync(
        int PrescriptionId, int MedicationId)
        => await _PrescriptionMedications
            .Include(PM => PM.Medication)
            .FirstOrDefaultAsync(PM =>
                PM.PrescriptionID == PrescriptionId &&
                PM.MedicationID == MedicationId);
}

// ══════════════════════════════════════════════════════════════════════════════
// MedicationRepository
// ══════════════════════════════════════════════════════════════════════════════
public class MedicationRepository : GenericRepository<Medication>, IMedicationRepository
{
    public MedicationRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<IEnumerable<Medication>> SearchByNameAsync(string Name)
        => await DbSet
            .Where(M => M.MedicationName.ToLower().Contains(Name.ToLower()))
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Medication>> GetByTypeAsync(string MedicationType)
        => await DbSet
            .Where(M => M.MedicationType.ToLower() == MedicationType.ToLower())
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> NameExistsAsync(string MedicationName)
        => await DbSet.AnyAsync(M => M.MedicationName.ToLower() == MedicationName.ToLower());
}

// ══════════════════════════════════════════════════════════════════════════════
// LabTestRepository
// ══════════════════════════════════════════════════════════════════════════════
public class LabTestRepository : GenericRepository<LabTest>, ILabTestRepository
{
    public LabTestRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<IEnumerable<LabTest>> GetByVisitIdAsync(int VisitId)
        => await DbSet
            .Where(L => L.VisitID == VisitId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<LabTest?> GetByIdWithVisitAsync(int LabTestId)
        => await DbSet
            .Include(L => L.Visit)
                .ThenInclude(V => V.Appointment)
                    .ThenInclude(A => A.Patient)
                        .ThenInclude(P => P.User)
            .FirstOrDefaultAsync(L => L.LabTestID == LabTestId);

    public async Task<bool> UpdateResultUrlAsync(int LabTestId, string ResultUrl)
    {
        var LabTest = await DbSet.FindAsync(LabTestId);
        if (LabTest is null) return false;
        LabTest.TestResultURL = ResultUrl;
        DbSet.Update(LabTest);
        return true;
    }
}
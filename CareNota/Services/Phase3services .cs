using AutoMapper;
using CareNota.DTOs.Diagnosis;
using CareNota.DTOs.LabTest;
using CareNota.DTOs.Medication;
using CareNota.DTOs.Prescription;
using CareNota.DTOs.Visit;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using CareNota.Services.Interfaces;


namespace CareNota.Services;

public class VisitService : IVisitService
{
    private readonly IVisitRepository _VisitRepo;
    private readonly IAppointmentRepository _AppointmentRepo;
    private readonly IMapper _Mapper;

    public VisitService(
        IVisitRepository VisitRepo,
        IAppointmentRepository AppointmentRepo,
        IMapper Mapper)
    {
        _VisitRepo = VisitRepo;
        _AppointmentRepo = AppointmentRepo;
        _Mapper = Mapper;
    }

    public async Task<IEnumerable<VisitDto>> GetAllAsync()
        => _Mapper.Map<IEnumerable<VisitDto>>(await _VisitRepo.GetAllAsync());

    public async Task<VisitDto?> GetByIdAsync(int VisitId)
    {
        var Visit = await _VisitRepo.GetByIdAsync(VisitId);
        return Visit is null ? null : _Mapper.Map<VisitDto>(Visit);
    }

    public async Task<VisitDetailDto?> GetDetailsAsync(int VisitId)
    {
        var Visit = await _VisitRepo.GetByIdWithDetailsAsync(VisitId);
        return Visit is null ? null : _Mapper.Map<VisitDetailDto>(Visit);
    }

    public async Task<IEnumerable<VisitDto>> GetByPatientIdAsync(int PatientId)
        => _Mapper.Map<IEnumerable<VisitDto>>(await _VisitRepo.GetByPatientIdAsync(PatientId));

    public async Task<VisitDto?> GetByAppointmentIdAsync(int AppointmentId)
    {
        var Visit = await _VisitRepo.GetByAppointmentIdAsync(AppointmentId);
        return Visit is null ? null : _Mapper.Map<VisitDto>(Visit);
    }

    public async Task<VisitDto> CreateAsync(CreateVisitDto Dto)
    {
        // 1. Validate appointment exists
        var Appointment = await _AppointmentRepo.GetByIdAsync(Dto.AppointmentID)
            ?? throw new KeyNotFoundException($"Appointment {Dto.AppointmentID} not found.");

        // 2. Block cancelled appointments
        if (Appointment.Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException(
                "Cannot create a visit for a cancelled appointment.");

        // 3. Block duplicate visits for the same appointment
        if (await _VisitRepo.GetByAppointmentIdAsync(Dto.AppointmentID) is not null)
            throw new InvalidOperationException(
                "This appointment already has a visit record.");

        // 4. Map DTO → Entity.
        //
        //    Two valid states after this mapping:
        //
        //    Manual path  → doctor filled SOAP fields in the request body.
        //                   Visit is created with clinical data already present.
        //                   Doctor can still update via PUT /visit/{id} anytime.
        //
        //    AI path      → doctor left SOAP fields empty (null).
        //                   Visit is created with Subjective/Objective/Assessment/Plan = null.
        //                   Doctor records audio → AI processes → doctor reviews →
        //                   POST /visits/{id}/summary/approve → SOAP fields written.
        //
        //    Both paths write to the same Visit row and the same SOAP columns.
        var Visit = _Mapper.Map<Visit>(Dto);

        await _VisitRepo.AddAsync(Visit);

        // 5. Mark appointment as Completed
        Appointment.Status = AppointmentStatus.Completed;
        _AppointmentRepo.Update(Appointment);

        // Both share the same DbContext — one save covers both changes
        await _VisitRepo.SaveChangesAsync();

        return _Mapper.Map<VisitDto>(Visit);
    }

    public async Task<VisitDto> UpdateAsync(int VisitId, UpdateVisitDto Dto)
    {
        var Visit = await _VisitRepo.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        // AutoMapper skips null fields (ForAllMembers condition in profile),
        // so only the fields the doctor actually sent will be updated.
        // Existing values for untouched fields are preserved.
        _Mapper.Map(Dto, Visit);

        _VisitRepo.Update(Visit);
        await _VisitRepo.SaveChangesAsync();

        return _Mapper.Map<VisitDto>(Visit);
    }

    public async Task DeleteAsync(int VisitId)
    {
        var Visit = await _VisitRepo.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        _VisitRepo.Remove(Visit);
        await _VisitRepo.SaveChangesAsync();
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// DiagnosisService
// ══════════════════════════════════════════════════════════════════════════════


public class DiagnosisService : IDiagnosisService
{
    private readonly IDiagnosisRepository _DiagnosisRepo;
    private readonly IVisitRepository _VisitRepo;
    private readonly IMapper _Mapper;

    public DiagnosisService(
        IDiagnosisRepository DiagnosisRepo,
        IVisitRepository VisitRepo,
        IMapper Mapper)
    {
        _DiagnosisRepo = DiagnosisRepo;
        _VisitRepo = VisitRepo;
        _Mapper = Mapper;
    }

    public async Task<IEnumerable<DiagnosisDto>> GetByVisitIdAsync(int VisitId)
    {
        var Diagnoses = await _DiagnosisRepo.GetByVisitIdAsync(VisitId);
        return _Mapper.Map<IEnumerable<DiagnosisDto>>(Diagnoses);
    }

    public async Task<DiagnosisDto> CreateAsync(CreateDiagnosisDto Dto)
    {
        if (!await _VisitRepo.ExistsAsync(V => V.VisitID == Dto.VisitID))
            throw new KeyNotFoundException($"Visit {Dto.VisitID} not found.");

        var Diagnosis = _Mapper.Map<Diagnosis>(Dto);
        await _DiagnosisRepo.AddAsync(Diagnosis);
        await _DiagnosisRepo.SaveChangesAsync();

        return _Mapper.Map<DiagnosisDto>(Diagnosis);
    }

    public async Task DeleteAsync(int DiagnosisId)
    {
        var Diagnosis = await _DiagnosisRepo.GetByIdAsync(DiagnosisId)
            ?? throw new KeyNotFoundException($"Diagnosis {DiagnosisId} not found.");

        _DiagnosisRepo.Remove(Diagnosis);
        await _DiagnosisRepo.SaveChangesAsync();
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// PrescriptionService
// ══════════════════════════════════════════════════════════════════════════════
public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _PrescriptionRepo;
    private readonly IVisitRepository _VisitRepo;
    private readonly IMedicationRepository _MedicationRepo;
    private readonly IMapper _Mapper;

    public PrescriptionService(
        IPrescriptionRepository PrescriptionRepo,
        IVisitRepository VisitRepo,
        IMedicationRepository MedicationRepo,
        IMapper Mapper)
    {
        _PrescriptionRepo = PrescriptionRepo;
        _VisitRepo = VisitRepo;
        _MedicationRepo = MedicationRepo;
        _Mapper = Mapper;
    }

    public async Task<PrescriptionDto?> GetByIdAsync(int PrescriptionId)
    {
        var Prescription = await _PrescriptionRepo.GetWithMedicationsAsync(PrescriptionId);
        return Prescription is null ? null : _Mapper.Map<PrescriptionDto>(Prescription);
    }

    public async Task<PrescriptionDto?> GetByVisitIdAsync(int VisitId)
    {
        var Prescription = await _PrescriptionRepo.GetByVisitIdAsync(VisitId);
        return Prescription is null ? null : _Mapper.Map<PrescriptionDto>(Prescription);
    }

    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto Dto)
    {
        if (!await _VisitRepo.ExistsAsync(V => V.VisitID == Dto.VisitID))
            throw new KeyNotFoundException($"Visit {Dto.VisitID} not found.");

        if (await _PrescriptionRepo.GetByVisitIdAsync(Dto.VisitID) is not null)
            throw new InvalidOperationException(
                "This visit already has a prescription. Update it instead.");

        var Prescription = _Mapper.Map<Prescription>(Dto);
        await _PrescriptionRepo.AddAsync(Prescription);
        await _PrescriptionRepo.SaveChangesAsync();

        return _Mapper.Map<PrescriptionDto>(Prescription);
    }

    public async Task<PrescriptionDto> UpdateAsync(int PrescriptionId, UpdatePrescriptionDto Dto)
    {
        var Prescription = await _PrescriptionRepo.GetByIdAsync(PrescriptionId)
            ?? throw new KeyNotFoundException($"Prescription {PrescriptionId} not found.");

        _Mapper.Map(Dto, Prescription);
        _PrescriptionRepo.Update(Prescription);
        await _PrescriptionRepo.SaveChangesAsync();

        var Updated = await _PrescriptionRepo.GetWithMedicationsAsync(PrescriptionId);
        return _Mapper.Map<PrescriptionDto>(Updated!);
    }

    public async Task<PrescriptionDto> AddMedicationAsync(
        int PrescriptionId, AddMedicationToPrescriptionDto Dto)
    {
        if (!await _PrescriptionRepo.ExistsAsync(P => P.PrescriptionID == PrescriptionId))
            throw new KeyNotFoundException($"Prescription {PrescriptionId} not found.");

        if (!await _MedicationRepo.ExistsAsync(M => M.MedicationID == Dto.MedicationID))
            throw new KeyNotFoundException($"Medication {Dto.MedicationID} not found.");

        if (await _PrescriptionRepo.GetPrescriptionMedicationAsync(PrescriptionId, Dto.MedicationID) is not null)
            throw new InvalidOperationException("This medication is already in the prescription.");

        var Pm = _Mapper.Map<PrescriptionMedication>(Dto);
        Pm.PrescriptionID = PrescriptionId;

        await _PrescriptionRepo.AddMedicationAsync(Pm);
        await _PrescriptionRepo.SaveChangesAsync();

        var Result = await _PrescriptionRepo.GetWithMedicationsAsync(PrescriptionId);
        return _Mapper.Map<PrescriptionDto>(Result!);
    }

    public async Task<PrescriptionDto> RemoveMedicationAsync(int PrescriptionId, int MedicationId)
    {
        var Entry = await _PrescriptionRepo.GetPrescriptionMedicationAsync(PrescriptionId, MedicationId)
            ?? throw new KeyNotFoundException(
                $"Medication {MedicationId} not found in prescription {PrescriptionId}.");

        await _PrescriptionRepo.RemoveMedicationAsync(PrescriptionId, MedicationId);
        await _PrescriptionRepo.SaveChangesAsync();

        var Result = await _PrescriptionRepo.GetWithMedicationsAsync(PrescriptionId);
        return _Mapper.Map<PrescriptionDto>(Result!);
    }

    public async Task DeleteAsync(int PrescriptionId)
    {
        var Prescription = await _PrescriptionRepo.GetByIdAsync(PrescriptionId)
            ?? throw new KeyNotFoundException($"Prescription {PrescriptionId} not found.");

        _PrescriptionRepo.Remove(Prescription);
        await _PrescriptionRepo.SaveChangesAsync();
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// MedicationService
// ══════════════════════════════════════════════════════════════════════════════
public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _MedicationRepo;
    private readonly IMapper _Mapper;

    public MedicationService(IMedicationRepository MedicationRepo, IMapper Mapper)
    {
        _MedicationRepo = MedicationRepo;
        _Mapper = Mapper;
    }

    public async Task<IEnumerable<MedicationDto>> GetAllAsync()
    {
        var Medications = await _MedicationRepo.GetAllAsync();
        return _Mapper.Map<IEnumerable<MedicationDto>>(Medications);
    }

    public async Task<MedicationDto?> GetByIdAsync(int MedicationId)
    {
        var Medication = await _MedicationRepo.GetByIdAsync(MedicationId);
        return Medication is null ? null : _Mapper.Map<MedicationDto>(Medication);
    }

    public async Task<IEnumerable<MedicationDto>> SearchByNameAsync(string Name)
    {
        var Results = await _MedicationRepo.SearchByNameAsync(Name);
        return _Mapper.Map<IEnumerable<MedicationDto>>(Results);
    }

    public async Task<IEnumerable<MedicationDto>> GetByTypeAsync(string Type)
    {
        var Results = await _MedicationRepo.GetByTypeAsync(Type);
        return _Mapper.Map<IEnumerable<MedicationDto>>(Results);
    }

    public async Task<MedicationDto> CreateAsync(CreateMedicationDto Dto)
    {
        if (await _MedicationRepo.NameExistsAsync(Dto.MedicationName))
            throw new InvalidOperationException(
                $"Medication '{Dto.MedicationName}' already exists.");

        var Medication = _Mapper.Map<Medication>(Dto);
        await _MedicationRepo.AddAsync(Medication);
        await _MedicationRepo.SaveChangesAsync();

        return _Mapper.Map<MedicationDto>(Medication);
    }

    public async Task<MedicationDto> UpdateAsync(int MedicationId, UpdateMedicationDto Dto)
    {
        var Medication = await _MedicationRepo.GetByIdAsync(MedicationId)
            ?? throw new KeyNotFoundException($"Medication {MedicationId} not found.");

        _Mapper.Map(Dto, Medication);
        _MedicationRepo.Update(Medication);
        await _MedicationRepo.SaveChangesAsync();

        return _Mapper.Map<MedicationDto>(Medication);
    }

    public async Task DeleteAsync(int MedicationId)
    {
        var Medication = await _MedicationRepo.GetByIdAsync(MedicationId)
            ?? throw new KeyNotFoundException($"Medication {MedicationId} not found.");

        _MedicationRepo.Remove(Medication);
        await _MedicationRepo.SaveChangesAsync();
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// LabTestService
// ══════════════════════════════════════════════════════════════════════════════
public class LabTestService : ILabTestService
{
    private readonly ILabTestRepository _LabTestRepo;
    private readonly IVisitRepository _VisitRepo;
    private readonly IMapper _Mapper;

    public LabTestService(
        ILabTestRepository LabTestRepo,
        IVisitRepository VisitRepo,
        IMapper Mapper)
    {
        _LabTestRepo = LabTestRepo;
        _VisitRepo = VisitRepo;
        _Mapper = Mapper;
    }

    public async Task<IEnumerable<LabTestDto>> GetByVisitIdAsync(int VisitId)
    {
        var LabTests = await _LabTestRepo.GetByVisitIdAsync(VisitId);
        return _Mapper.Map<IEnumerable<LabTestDto>>(LabTests);
    }

    public async Task<LabTestDto?> GetByIdAsync(int LabTestId)
    {
        var LabTest = await _LabTestRepo.GetByIdWithVisitAsync(LabTestId);
        return LabTest is null ? null : _Mapper.Map<LabTestDto>(LabTest);
    }

    public async Task<LabTestDto> CreateAsync(CreateLabTestDto Dto)
    {
        if (!await _VisitRepo.ExistsAsync(V => V.VisitID == Dto.VisitID))
            throw new KeyNotFoundException($"Visit {Dto.VisitID} not found.");

        var LabTest = _Mapper.Map<LabTest>(Dto);
        await _LabTestRepo.AddAsync(LabTest);
        await _LabTestRepo.SaveChangesAsync();

        return _Mapper.Map<LabTestDto>(LabTest);
    }

    public async Task<LabTestDto> UploadResultAsync(
        int LabTestId, IFormFile File, string UploadFolder)
    {
        var LabTest = await _LabTestRepo.GetByIdAsync(LabTestId)
            ?? throw new KeyNotFoundException($"Lab test {LabTestId} not found.");

        // Validate extension
        var AllowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
        var Extension = Path.GetExtension(File.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(Extension))
            throw new ArgumentException("Only PDF, PNG, JPG files are allowed.");

        Directory.CreateDirectory(UploadFolder);

        // Delete old file if exists
        if (!string.IsNullOrEmpty(LabTest.TestResultURL) &&
            System.IO.File.Exists(LabTest.TestResultURL))
            System.IO.File.Delete(LabTest.TestResultURL);

        // Save new file with unique name
        var FileName = $"LabTest_{LabTestId}_{Guid.NewGuid():N}{Extension}";
        var FilePath = Path.Combine(UploadFolder, FileName);

        await using var Stream = new FileStream(FilePath, FileMode.Create);
        await File.CopyToAsync(Stream);

        var Updated = await _LabTestRepo.UpdateResultUrlAsync(LabTestId, FilePath);
        if (!Updated)
            throw new InvalidOperationException("Failed to update lab test result.");

        await _LabTestRepo.SaveChangesAsync();

        var Result = await _LabTestRepo.GetByIdWithVisitAsync(LabTestId);
        return _Mapper.Map<LabTestDto>(Result!);
    }

    public async Task DeleteAsync(int LabTestId)
    {
        var LabTest = await _LabTestRepo.GetByIdAsync(LabTestId)
            ?? throw new KeyNotFoundException($"Lab test {LabTestId} not found.");

        if (!string.IsNullOrEmpty(LabTest.TestResultURL) &&
            System.IO.File.Exists(LabTest.TestResultURL))
            System.IO.File.Delete(LabTest.TestResultURL);

        _LabTestRepo.Remove(LabTest);
        await _LabTestRepo.SaveChangesAsync();
    }
}
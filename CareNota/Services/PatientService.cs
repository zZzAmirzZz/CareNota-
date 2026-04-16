using AutoMapper;
using CareNota.DTOs.Patient;
using CareNota.Repositories.Interfaces;
using CareNota.Services.Interfaces;

namespace CareNota.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _PatientRepo;
    private readonly IMapper _Mapper;

    public PatientService(IPatientRepository PatientRepo, IMapper Mapper)
    {
        _PatientRepo = PatientRepo;
        _Mapper = Mapper;
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var Patients = await _PatientRepo.GetAllAsync();
        return _Mapper.Map<IEnumerable<PatientDto>>(Patients);
    }

    public async Task<PatientDto?> GetByIdAsync(int PatientId)
    {
        var Patient = await _PatientRepo.GetByIdAsync(PatientId);
        return Patient is null ? null : _Mapper.Map<PatientDto>(Patient);
    }

    public async Task<PatientDetailDto?> GetDetailsAsync(int PatientId)
    {
        // Uses GetWithMedicalHistoryAsync so navigation properties are already loaded
        var Patient = await _PatientRepo.GetWithMedicalHistoryAsync(PatientId);
        return Patient is null ? null : _Mapper.Map<PatientDetailDto>(Patient);
    }

    public async Task<IEnumerable<PatientDto>> SearchByNameAsync(string Name)
    {
        var Patients = await _PatientRepo.SearchByNameAsync(Name);
        return _Mapper.Map<IEnumerable<PatientDto>>(Patients);
    }

    // ── Write ─────────────────────────────────────────────────────────────────

    public async Task<PatientDto> UpdateAsync(int PatientId, UpdatePatientDto Dto)
    {
        var Patient = await _PatientRepo.GetByIdAsync(PatientId)
            ?? throw new KeyNotFoundException($"Patient {PatientId} not found.");

        // AutoMapper updates only the mapped fields; ignores UserId, nav props, etc.
        _Mapper.Map(Dto, Patient);
        _PatientRepo.Update(Patient);
        await _PatientRepo.SaveChangesAsync();

        // Re-fetch with User navigation so FullName/Email are available for the response
        var Updated = await _PatientRepo.GetWithMedicalHistoryAsync(PatientId);
        return _Mapper.Map<PatientDto>(Updated!);
    }

    public async Task DeleteAsync(int PatientId)
    {
        var Patient = await _PatientRepo.GetByIdAsync(PatientId)
            ?? throw new KeyNotFoundException($"Patient {PatientId} not found.");

        _PatientRepo.Remove(Patient);
        await _PatientRepo.SaveChangesAsync();
    }
}
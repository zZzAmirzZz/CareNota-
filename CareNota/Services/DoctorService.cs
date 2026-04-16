using AutoMapper;
using CareNota.DTOs.Doctor;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _DoctorRepo;
    private readonly IMapper _Mapper;

    public DoctorService(IDoctorRepository DoctorRepo, IMapper Mapper)
    {
        _DoctorRepo = DoctorRepo;
        _Mapper = Mapper;
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var Doctors = await _DoctorRepo.GetAllAsync();
        return _Mapper.Map<IEnumerable<DoctorDto>>(Doctors);
    }

    public async Task<DoctorDto?> GetByIdAsync(int DoctorId)
    {
        var Doctor = await _DoctorRepo.GetByIdAsync(DoctorId);
        return Doctor is null ? null : _Mapper.Map<DoctorDto>(Doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetBySpecialtyAsync(string Specialty)
    {
        var Doctors = await _DoctorRepo.GetBySpecialtyAsync(Specialty);
        return _Mapper.Map<IEnumerable<DoctorDto>>(Doctors);
    }

    // ── Write ─────────────────────────────────────────────────────────────────

    public async Task<DoctorDto> UpdateAsync(int DoctorId, UpdateDoctorDto Dto)
    {
        var Doctor = await _DoctorRepo.GetByIdAsync(DoctorId)
            ?? throw new KeyNotFoundException($"Doctor {DoctorId} not found.");

        _Mapper.Map(Dto, Doctor);
        _DoctorRepo.Update(Doctor);
        await _DoctorRepo.SaveChangesAsync();

        // Re-fetch with User navigation so FullName/Email are in the response
        var Updated = await _DoctorRepo.GetByIdAsync(DoctorId);
        return _Mapper.Map<DoctorDto>(Updated!);
    }

    public async Task DeleteAsync(int DoctorId)
    {
        var Doctor = await _DoctorRepo.GetByIdAsync(DoctorId)
            ?? throw new KeyNotFoundException($"Doctor {DoctorId} not found.");

        _DoctorRepo.Remove(Doctor);
        await _DoctorRepo.SaveChangesAsync();
    }
}
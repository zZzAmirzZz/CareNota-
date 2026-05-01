using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories;
using Microsoft.EntityFrameworkCore;



public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
   
    public async Task<IEnumerable<Doctor>> GetBySpecialtyAsync(string specialty)
    {
        return await _context.Doctors
                             .Where(d => d.Specialty == specialty)
                             .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int doctorId)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.DoctorID == doctorId);
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .Include(d => d.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Doctor?> GetByUserIdAsync(string userId)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

}
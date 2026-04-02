using CareNota.Data;
using CareNota.Models;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public List<Doctor> GetDoctorsBySpecialty(string specialty)
    {
        return _context.Doctors
                       .Where(d => d.Specialty == specialty)
                       .ToList();
    }
}
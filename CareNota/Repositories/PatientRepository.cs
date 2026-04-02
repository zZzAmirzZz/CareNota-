using CareNota.Models;
using CareNota.Data;


public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public List<Patient> GetPatientsByGender(string gender)
    {
        return _context.Patients.Where(p => p.Gender == gender).ToList();
    }
}
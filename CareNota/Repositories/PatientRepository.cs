using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories;
using Microsoft.EntityFrameworkCore;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public List<Patient> GetPatientsByGender(string gender)
    {
        return Context.Patients
            .Where(p => p.Gender == gender)
            .ToList();
    }
}
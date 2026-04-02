using CareNota.Models;

public interface IPatientRepository
{
    public interface IPatientRepository : IRepository<Patient>
    {
        List<Patient> GetPatientsByGender(string gender);
    }
}
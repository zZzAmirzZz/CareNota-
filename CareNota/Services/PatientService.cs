using CareNota.Models;

public class PatientService : IPatientService
{
    private readonly IRepository<Patient> _repo;

    public PatientService(IRepository<Patient> repo)
    {
        _repo = repo;
    }

    public List<Patient> GetAll() => _repo.GetAll();

    public Patient GetById(int id) => _repo.GetById(id);

    public void Add(Patient patient) => _repo.Add(patient);

    public void Update(Patient patient) => _repo.Update(patient);

    public void Delete(int id) => _repo.Delete(id);
}
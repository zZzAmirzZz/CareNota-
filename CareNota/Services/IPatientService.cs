using CareNota.Models;

public interface IPatientService
{
    List<Patient> GetAll();
    Patient GetById(int id);
    void Add(Patient patient);
    void Update(Patient patient);
    void Delete(int id);
}
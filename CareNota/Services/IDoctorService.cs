using CareNota.Models;

public interface IDoctorService
{
    List<Doctor> GetAll();
    Doctor GetById(int id);
    void Add(Doctor doctor);
    void Update(Doctor doctor);
    void Delete(int id);
    List<Doctor> GetDoctorsBySpecialty(string specialty);
}
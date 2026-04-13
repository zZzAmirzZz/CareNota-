//using CareNota.Models;

//public class DoctorService : IDoctorService
//{
//    private readonly IDoctorRepository _repo;

//    public DoctorService(IDoctorRepository repo)
//    {
//        _repo = repo;
//    }

//    public List<Doctor> GetAll() => _repo.GetAll();

//    public Doctor GetById(int id) => _repo.GetById(id);

//    public void Add(Doctor doctor) => _repo.Add(doctor);

//    public void Update(Doctor doctor) => _repo.Update(doctor);

//    public void Delete(int id) => _repo.Delete(id);

//    public List<Doctor> GetDoctorsBySpecialty(string specialty)
//        => _repo.GetDoctorsBySpecialty(specialty);
//}
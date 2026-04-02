using CareNota.Models;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repo;

    public AppointmentService(IAppointmentRepository repo)
    {
        _repo = repo;
    }

    public List<Appointment> GetAll() => _repo.GetAll();

    public Appointment GetById(int id) => _repo.GetById(id);

    public void Add(Appointment appointment) => _repo.Add(appointment);

    public void Update(Appointment appointment) => _repo.Update(appointment);

    public void Delete(int id) => _repo.Delete(id);

    public List<Appointment> GetAppointmentsByDate(DateTime date)
        => _repo.GetAppointmentsByDate(date);
}
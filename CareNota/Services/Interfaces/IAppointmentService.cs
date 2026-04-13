using CareNota.Models;

public interface IAppointmentService
{
    List<Appointment> GetAll();
    Appointment GetById(int id);
    void Add(Appointment appointment);
    void Update(Appointment appointment);
    void Delete(int id);
    List<Appointment> GetAppointmentsByDate(DateTime date);
}
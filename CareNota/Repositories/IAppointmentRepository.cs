using CareNota.Models;
using CareNota.Data;
using Microsoft.EntityFrameworkCore;

public interface IAppointmentRepository : IRepository<Appointment>
{
    List<Appointment> GetAppointmentsByDate(DateTime date);
}
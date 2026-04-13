using CareNota.Models;
using CareNota.Data;
using CareNota.Repositories;
public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public List<Appointment> GetAppointmentsByDate(DateTime date)
    {
        return _context.Appointments
                       .Where(a => a.AppointmentDate.Date == date.Date)
                       .ToList();
    }
}
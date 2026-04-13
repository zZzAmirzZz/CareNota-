using CareNota.Models;
using CareNota.Data;
using Microsoft.EntityFrameworkCore;

public interface IDoctorRepository : IRepository<Doctor>
{
    List<Doctor> GetDoctorsBySpecialty(string specialty);
}


using CareNota.Models;
using Microsoft.EntityFrameworkCore;
namespace CareNota.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
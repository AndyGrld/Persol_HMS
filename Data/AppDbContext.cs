using Microsoft.EntityFrameworkCore;
using Persol_HMS.Controllers;

namespace Persol_HMS;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}

    public AppDbContext(){}

    public DbSet<Staff> Staff { get; set; }
    public DbSet<Patient> Patients { get; set; }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persol_Hms.Models;

namespace Persol_HMS.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Lab> Labs { get; set; }
    public DbSet<Medicals> Medicals { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Symptoms> Symptoms { get; set; }
    public DbSet<Vitals> Vitals { get; set; }
    public DbSet<Users> Staff { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Drugs> Drugs { get; set; }
    public DbSet<Department> Departments { get; set; }

}
